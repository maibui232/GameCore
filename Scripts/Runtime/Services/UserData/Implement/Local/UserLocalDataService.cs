namespace GameCore.Services.UserData.Implement.Local
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Cysharp.Threading.Tasks;
    using GameCore.Services.Logger;
    using GameCore.Services.UserData.Interface;
    using Newtonsoft.Json;
    using UnityEngine;

    public class UserUserDataService : IUserDataService
    {
        private const string LocalDataPrefixKey = "LD_";

#region Inject

        private readonly Dictionary<Type, IUserData> typeToLocalDataCache;

#endregion

        public UserUserDataService
        (
            IEnumerable<IUserData> localDataEnumerable
        )
        {
            this.typeToLocalDataCache = localDataEnumerable.ToDictionary(x => x.GetType(), x => x);
        }

        public UniTask Save<T>() where T : IUserData
        {
            return this.InternalSave(typeof(T));
        }

        public async UniTask<T> Load<T>() where T : IUserData
        {
            await this.InternalLoad(typeof(T));

            return (T)this.typeToLocalDataCache[typeof(T)];
        }

        public UniTask SaveAll()
        {
            return UniTask.WhenAll(this.typeToLocalDataCache.Select(pair => this.InternalSave(pair.Key)));
        }

        public UniTask LoadAll()
        {
            return UniTask.WhenAll(this.typeToLocalDataCache.Select(pair => this.InternalLoad(pair.Key)));
        }

        private string GetLocalDataKey(MemberInfo type)
        {
            return $"{LocalDataPrefixKey}{type.Name}";
        }

        private UniTask InternalSave(Type type)
        {
            if (!this.typeToLocalDataCache.TryGetValue(type, out var data))
            {
                LoggerUtils.Error($"Doesn't contain or implement type: {type.Name} in {this.GetType().Name}");

                return UniTask.CompletedTask;
            }

            var jsonData = JsonConvert.SerializeObject(data);
            PlayerPrefs.SetString(this.GetLocalDataKey(type), jsonData);
            LoggerUtils.Log($"UserLocalData Save: {this.GetLocalDataKey(type)}", Color.green);

            return UniTask.CompletedTask;
        }

        private UniTask InternalLoad(Type type)
        {
            if (!this.typeToLocalDataCache.TryGetValue(type, out _))
            {
                LoggerUtils.Error($"Doesn't contain or implement type: {type.Name} in {this.GetType().Name}");

                return UniTask.CompletedTask;
            }

            if (!PlayerPrefs.HasKey(this.GetLocalDataKey(type)))
            {
                this.typeToLocalDataCache[type].Init();

                return UniTask.CompletedTask;
            }

            var jsonData = PlayerPrefs.GetString(this.GetLocalDataKey(type));
            var data     = JsonConvert.DeserializeObject(jsonData, type);

            if (data is not IUserData d) return UniTask.CompletedTask;
            this.typeToLocalDataCache[type] = d;
            LoggerUtils.Log($"UserLocalData Load: {this.GetLocalDataKey(type)}", Color.green);

            return UniTask.CompletedTask;
        }
    }
}