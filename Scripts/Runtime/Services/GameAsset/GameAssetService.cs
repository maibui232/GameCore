namespace GameCore.Services.GameAsset
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;

    public interface IGameAssetService
    {
        AsyncOperationHandle<T> LoadAssetAsync<T>(object key) where T : Object;
        void                    ReleaseAsset(object key);
        void                    ReleaseAsset(AsyncOperationHandle asyncOperationHandle);
        void                    ReleaseAsset<T>(AsyncOperationHandle<T> asyncOperationHandle);
    }

    public class GameAssetService : IGameAssetService
    {
        private readonly Dictionary<object, AsyncOperationHandle> keyToCacheAsyncOperationHandle = new();

        #region Load Asset

        public AsyncOperationHandle<T> LoadAssetAsync<T>(object key) where T : Object
        {
            if (this.keyToCacheAsyncOperationHandle.TryGetValue(key, out var operationHandle))
            {
                return operationHandle.Convert<T>();
            }

            var asyncOperationHandle = Addressables.LoadAssetAsync<T>(key);
            this.keyToCacheAsyncOperationHandle.Add(key, asyncOperationHandle);
            return asyncOperationHandle;
        }

        #endregion

        #region Release Asset

        public void ReleaseAsset(object key)
        {
            if (!this.keyToCacheAsyncOperationHandle.TryGetValue(key, out var operationHandle)) return;

            Addressables.Release(operationHandle);
            this.keyToCacheAsyncOperationHandle.Remove(key);
        }

        public void ReleaseAsset<T>(AsyncOperationHandle<T> asyncOperationHandle)
        {
            if (this.keyToCacheAsyncOperationHandle.ContainsValue(asyncOperationHandle))
            {
                var key = this.keyToCacheAsyncOperationHandle.First(keyPair => keyPair.Value.Convert<T>().Equals(asyncOperationHandle));
                this.keyToCacheAsyncOperationHandle.Remove(key);
            }

            Addressables.Release(asyncOperationHandle);
        }

        public void ReleaseAsset(AsyncOperationHandle asyncOperationHandle)
        {
            if (this.keyToCacheAsyncOperationHandle.ContainsValue(asyncOperationHandle))
            {
                var key = this.keyToCacheAsyncOperationHandle.First(keyPair => keyPair.Value.Equals(asyncOperationHandle));
                this.keyToCacheAsyncOperationHandle.Remove(key);
            }

            Addressables.Release(asyncOperationHandle);
        }

        #endregion
    }
}