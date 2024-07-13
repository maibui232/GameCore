namespace GameCore.Services.GameAsset
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.SceneManagement;

    public interface IGameAssetService
    {
        AsyncOperationHandle<T> LoadAssetAsync<T>(object                key,                  string targetScene = "") where T : Object;
        void                    ReleaseAsset(object                     key,                  string targetScene = "");
        void                    ReleaseAsset(AsyncOperationHandle       asyncOperationHandle, string targetScene = "");
        void                    ReleaseAsset<T>(AsyncOperationHandle<T> asyncOperationHandle, string targetScene = "");
        void                    ReleaseScene(string                     sceneName);
    }

    public class GameAssetService : IGameAssetService
    {
        private readonly Dictionary<object, AsyncOperationHandle> keyToCacheAsyncOperationHandle = new();
        private readonly Dictionary<string, List<object>>         sceneNameToAssetsLoaded        = new();

        private string GetSceneName(string sceneName)
        {
            return string.IsNullOrEmpty(sceneName) ? SceneManager.GetActiveScene().name : sceneName;
        }

        private void AddAssetKey(object key, string targetScene)
        {
            var sceneName = this.GetSceneName(targetScene);
            if (this.sceneNameToAssetsLoaded.TryGetValue(sceneName, out var loadedAssets))
            {
                loadedAssets.Add(key);
            }
            else
            {
                this.sceneNameToAssetsLoaded.Add(sceneName, new List<object> { key });
            }
        }

        private void RemoveAssetKey(object key, string targetScene)
        {
            var sceneName = this.GetSceneName(targetScene);
            if (this.sceneNameToAssetsLoaded.TryGetValue(sceneName, out var loadedAssets))
            {
                loadedAssets.Remove(key);
            }
        }

#region Load Asset

        public AsyncOperationHandle<T> LoadAssetAsync<T>(object key, string targetScene = "") where T : Object
        {
            if (this.keyToCacheAsyncOperationHandle.TryGetValue(key, out var operationHandle))
                return operationHandle.Convert<T>();

            var asyncOperationHandle = Addressables.LoadAssetAsync<T>(key);
            this.keyToCacheAsyncOperationHandle.Add(key, asyncOperationHandle);
            this.AddAssetKey(key, targetScene);

            return asyncOperationHandle;
        }

#endregion

#region Release Asset

        public void ReleaseAsset(object key, string targetScene = "")
        {
            if (!this.keyToCacheAsyncOperationHandle.TryGetValue(key, out var operationHandle)) return;

            Addressables.Release(operationHandle);
            this.keyToCacheAsyncOperationHandle.Remove(key);
            this.RemoveAssetKey(key, targetScene);
        }

        public void ReleaseAsset(AsyncOperationHandle asyncOperationHandle, string targetScene = "")
        {
            if (this.keyToCacheAsyncOperationHandle.ContainsValue(asyncOperationHandle))
            {
                var key = this.keyToCacheAsyncOperationHandle.First(keyPair => keyPair.Value.Equals(asyncOperationHandle));
                this.keyToCacheAsyncOperationHandle.Remove(key);
                this.RemoveAssetKey(key, targetScene);
            }

            Addressables.Release(asyncOperationHandle);
        }

        public void ReleaseAsset<T>(AsyncOperationHandle<T> asyncOperationHandle, string targetScene = "")
        {
            if (this.keyToCacheAsyncOperationHandle.ContainsValue(asyncOperationHandle))
            {
                var key = this.keyToCacheAsyncOperationHandle.First(keyPair => keyPair.Value.Convert<T>().Equals(asyncOperationHandle));
                this.keyToCacheAsyncOperationHandle.Remove(key);
                this.RemoveAssetKey(key, targetScene);
            }

            Addressables.Release(asyncOperationHandle);
        }

        public void ReleaseScene(string targetScene = "")
        {
            var sceneName = this.GetSceneName(targetScene);
            if (!this.sceneNameToAssetsLoaded.TryGetValue(sceneName, out var loadedAssetKey)) return;

            var assetsKey = loadedAssetKey.ToList();
            foreach (var key in assetsKey)
            {
                this.ReleaseAsset(key);
            }

            this.sceneNameToAssetsLoaded.Remove(sceneName);
        }

#endregion
    }
}