namespace GameCore.Services.SceneFlow
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using GameCore.Services.Logger;
    using GameCore.Services.Message;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;

    public interface ISceneFlowService
    {
        SceneInstance                       MainActiveScene { get; }
        List<SceneInstance>                 OverlayScene    { get; }
        UniTask<SceneInstance>              OpenSingleSceneAsync(string    sceneName,     bool activeOnLoad     = true);
        UniTask<SceneInstance>              OpenAdditiveSceneAsync(string  sceneName,     bool activeOnLoad     = true);
        AsyncOperationHandle<SceneInstance> UnloadSceneAsync(SceneInstance sceneInstance, bool autoReleaseAsset = true);
    }

    public class SceneFlowService : ISceneFlowService
    {
#region Inject

        private readonly IMessageService messageService;

#endregion

        public SceneFlowService(IMessageService messageService)
        {
            this.messageService = messageService;
        }

        public SceneInstance       MainActiveScene { get; private set; }
        public List<SceneInstance> OverlayScene    { get; } = new();

        public async UniTask<SceneInstance> OpenSingleSceneAsync(string sceneName, bool activeOnLoad = true)
        {
            // release scene
            foreach (var overlayScene in this.OverlayScene) this.UnloadSceneAsync(overlayScene);

            if (this.MainActiveScene.Scene.IsValid()) this.UnloadSceneAsync(this.MainActiveScene);

            this.MainActiveScene = default;
            this.OverlayScene.Clear();

            // load new scene
            var sceneInstance = await Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single, activeOnLoad);
            this.openSingleSceneMessage.SceneName = sceneName;
            this.messageService.Publish(this.openSingleSceneMessage);

            LoggerService.Log($"Load new single scene: {sceneInstance.Scene.name}", Color.magenta);
            this.MainActiveScene = sceneInstance;

            return sceneInstance;
        }

        public async UniTask<SceneInstance> OpenAdditiveSceneAsync(string sceneName, bool activeOnLoad = true)
        {
            var sceneInstance = await Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Additive, activeOnLoad);
            this.openAdditiveSceneMessage.SceneName = sceneName;
            this.messageService.Publish(this.openAdditiveSceneMessage);

            LoggerService.Log($"Load new additive scene: {sceneInstance.Scene.name}", Color.magenta);
            this.OverlayScene.Add(sceneInstance);

            return sceneInstance;
        }

        public AsyncOperationHandle<SceneInstance> UnloadSceneAsync
            (SceneInstance sceneInstance, bool autoReleaseAsset = true)
        {
            if (this.OverlayScene.Contains(sceneInstance)) this.OverlayScene.Remove(sceneInstance);

            this.releaseSceneMessage.SceneName = sceneInstance.Scene.name;
            this.messageService.Publish(this.releaseSceneMessage);

            LoggerService.Log($"Release scene: {sceneInstance.Scene.name}", Color.magenta);

            return Addressables.UnloadSceneAsync(sceneInstance, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects,
                                                 autoReleaseAsset);
        }

#region Cache

        private readonly OpenAdditiveSceneMessage openAdditiveSceneMessage = new();
        private readonly OpenSingleSceneMessage   openSingleSceneMessage   = new();
        private readonly ReleaseSceneMessage      releaseSceneMessage      = new();

#endregion
    }
}