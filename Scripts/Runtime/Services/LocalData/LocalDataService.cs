namespace GameCore.Services.LocalData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using GameCore.Services.LocalData.Interface;
    using GameCore.Services.Logger;
    using Newtonsoft.Json;
    using UnityEngine;

    public interface ILocalDataService
    {
        void Save<T>() where T : ILocalData;
        void Load<T>() where T : ILocalData;
        void SaveAll();
        void LoadAll();
    }

    public class LocalDataService : ILocalDataService
    {
        #region Inject

        private readonly Dictionary<Type, ILocalData> typeToLocalDataCache;
        private readonly ILoggerService               loggerService;

        #endregion

        private const string LocalDataPrefixKey = "LD_";

        private string GetLocalDataKey(MemberInfo type) { return $"{LocalDataPrefixKey}{type.Name}"; }

        public LocalDataService
        (
            IEnumerable<ILocalData> localDataEnumerable,
            ILoggerService loggerService
        )
        {
            this.typeToLocalDataCache = localDataEnumerable.ToDictionary(x => x.GetType(), x => x);
            this.loggerService        = loggerService;
        }

        public void Save<T>() where T : ILocalData { this.InternalSave(typeof(T)); }

        private void InternalSave(Type type)
        {
            if (!this.typeToLocalDataCache.TryGetValue(type, out var data))
            {
                this.loggerService.Error($"Doesn't contain or implement type: {type.Name} in {this.GetType().Name}");
                return;
            }

            var jsonData = JsonConvert.SerializeObject(data);
            PlayerPrefs.SetString(this.GetLocalDataKey(type), jsonData);
            this.loggerService.Log($"Save: {this.GetLocalDataKey(type)}", Color.green);
        }

        public void Load<T>() where T : ILocalData { this.InternalLoad(typeof(T)); }

        private void InternalLoad(Type type)
        {
            if (!this.typeToLocalDataCache.TryGetValue(type, out _))
            {
                this.loggerService.Error($"Doesn't contain or implement type: {type.Name} in {this.GetType().Name}");
                return;
            }

            if (!PlayerPrefs.HasKey(this.GetLocalDataKey(type)))
            {
                this.typeToLocalDataCache[type].Init();
                return;
            }
            
            var jsonData = PlayerPrefs.GetString(this.GetLocalDataKey(type));
            var data     = JsonConvert.DeserializeObject(jsonData, type);
            if(data is not ILocalData d) return;
            this.typeToLocalDataCache[type] = d;
            this.loggerService.Log($"Load: {this.GetLocalDataKey(type)}", Color.green);
        }

        public void SaveAll()
        {
            foreach (var (key, value) in this.typeToLocalDataCache)
            {
                this.InternalSave(key);
            }
        }

        public void LoadAll()
        {
            foreach (var (key, value) in this.typeToLocalDataCache)
            {
                this.InternalLoad(key);
            }
        }
    }
}