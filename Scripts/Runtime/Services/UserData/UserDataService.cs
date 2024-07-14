namespace GameCore.Services.UserData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using GameCore.Services.Logger;
    using GameCore.Services.UserData.Interface;
    using Newtonsoft.Json;
    using UnityEngine;

    public interface ILocalDataService
    {
        void Save<T>() where T : IUserData;
        void Load<T>() where T : IUserData;
        void SaveAll();
        void LoadAll();
    }

    public class UserDataService : ILocalDataService
    {
        private const string LocalDataPrefixKey = "LD_";

#region Inject

        private readonly Dictionary<Type, IUserData> typeToLocalDataCache;

#endregion

        public UserDataService
        (
            IEnumerable<IUserData> localDataEnumerable
        )
        {
            this.typeToLocalDataCache = localDataEnumerable.ToDictionary(x => x.GetType(), x => x);
        }

        public void Save<T>() where T : IUserData
        {
            this.InternalSave(typeof(T));
        }

        public void Load<T>() where T : IUserData
        {
            this.InternalLoad(typeof(T));
        }

        public void SaveAll()
        {
            foreach (var (key, value) in this.typeToLocalDataCache) this.InternalSave(key);
        }

        public void LoadAll()
        {
            foreach (var (key, value) in this.typeToLocalDataCache) this.InternalLoad(key);
        }

        private string GetLocalDataKey(MemberInfo type)
        {
            return $"{LocalDataPrefixKey}{type.Name}";
        }

        private void InternalSave(Type type)
        {
            if (!this.typeToLocalDataCache.TryGetValue(type, out var data))
            {
                LoggerService.Error($"Doesn't contain or implement type: {type.Name} in {this.GetType().Name}");

                return;
            }

            var jsonData = JsonConvert.SerializeObject(data);
            PlayerPrefs.SetString(this.GetLocalDataKey(type), jsonData);
            LoggerService.Log($"Save: {this.GetLocalDataKey(type)}", Color.green);
        }

        private void InternalLoad(Type type)
        {
            if (!this.typeToLocalDataCache.TryGetValue(type, out _))
            {
                LoggerService.Error($"Doesn't contain or implement type: {type.Name} in {this.GetType().Name}");

                return;
            }

            if (!PlayerPrefs.HasKey(this.GetLocalDataKey(type)))
            {
                this.typeToLocalDataCache[type].Init();

                return;
            }

            var jsonData = PlayerPrefs.GetString(this.GetLocalDataKey(type));
            var data     = JsonConvert.DeserializeObject(jsonData, type);

            if (data is not IUserData d) return;
            this.typeToLocalDataCache[type] = d;
            LoggerService.Log($"Load: {this.GetLocalDataKey(type)}", Color.green);
        }
    }
}