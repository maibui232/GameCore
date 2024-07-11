namespace GameCore.Services.ScreenFlow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Cysharp.Threading.Tasks;
    using GameCore.Attribute;
    using GameCore.Extensions.VContainer;
    using GameCore.Services.GameAsset;
    using GameCore.Services.Logger;
    using GameCore.Services.Message;
    using GameCore.Services.SceneFlow;
    using GameCore.Services.ScreenFlow.Base;
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;

    public interface IScreenFlowService
    {
        RootUIView          RootUIView { get; set; }
        UniTask<TPresenter> InitScreenManually<TPresenter>() where TPresenter : IScreenPresenter;
        UniTask<TPresenter> InitScreenManually<TPresenter, TModel>(TModel model) where TPresenter : IScreenPresenter<TModel>;
        UniTask<TPresenter> OpenScreenAsync<TPresenter>() where TPresenter : IScreenPresenter;
        UniTask<TPresenter> OpenScreenAsync<TPresenter, TModel>(TModel model) where TPresenter : IScreenPresenter<TModel>;
        UniTask             CloseCurrentScreenAsync();
        UniTask             CloseAllScreenAsync();
        UniTask             DestroyCurrentScreenAsync();
        UniTask             DestroyAllScreenAsync();
    }

    public class ScreenFlowService : IScreenFlowService
    {
        public ScreenFlowService
        (
            IObjectResolver   resolver,
            IGameAssetService gameAssetService,
            IMessageService   messageService
        )
        {
            this.resolver         = resolver;
            this.gameAssetService = gameAssetService;
            this.messageService   = messageService;

            this.OnInitialize();
        }

        private void OnInitialize()
        {
            this.messageService.Subscribe<OpenSingleSceneMessage>(this.OnOpenSingleScene);
            this.messageService.Subscribe<ReleaseSceneMessage>(this.OnReleaseScene);
            this.messageService.Subscribe<ScreenInitializeMessage>(this.OnScreenInitialize);
            this.messageService.Subscribe<ScreenOpenedMessage>(this.OnScreenOpened);
            this.messageService.Subscribe<ScreenClosedMessage>(this.OnScreenClosed);
            this.messageService.Subscribe<ScreenDestroyedMessage>(this.OnScreenDestroyed);
        }

        private async UniTask CloseAllScreenIfOverlap(UIInfoAttribute uiInfo)
        {
            if (uiInfo.Overlap) return;
            if (this.orderLayerToScreenShow.TryGetValue(uiInfo.OrderLayer, out var screenPresenters))
            {
                var listScreen = screenPresenters.ToList();
                await UniTask.WhenAll(listScreen.Select(item => item.CloseViewAsync()));
            }
        }

        private async UniTask<TPresenter> GetOrAddScreen<TPresenter>(IUIView viewParam = null) where TPresenter : IScreenPresenter
        {
            var uiInfo = this.GetUIInfo<TPresenter>();

            if (uiInfo == null) throw new Exception($"Could not find screen with addressable id: {nameof(UIInfoAttribute)}");

            await this.CloseAllScreenIfOverlap(uiInfo);

            var hasCachedPresenter = this.cachedScreens.TryGetValue(uiInfo.AddressableId, out var outPresenter);
            var presenter = hasCachedPresenter
                                ? (TPresenter)outPresenter
                                : (TPresenter)VContainerExtensions.ContainerBuilder.Register<TPresenter>(Lifetime.Singleton).As().Build().SpawnInstance(this.resolver);

            var parent = this.RootUIView.GetOrCreateOverlayCanvas((int)uiInfo.OrderLayer).transform;
            if (!hasCachedPresenter)
            {
                this.cachedScreens.Add(uiInfo.AddressableId, presenter);
                var prefab = await this.gameAssetService.LoadAssetAsync<GameObject>(uiInfo.AddressableId);
                var view   = viewParam ?? this.resolver.Instantiate(prefab, parent).GetComponent<IUIView>();

                presenter.SetView(view);
                presenter.InitView();
            }

            presenter.SetParent(parent);
            this.CurrentScreen = presenter;
            await presenter.OpenViewAsync();

            return presenter;
        }

        private UIInfoAttribute GetUIInfo<TPresenter>() where TPresenter : IScreenPresenter
        {
            return this.GetUIInfo(typeof(TPresenter));
        }

        private UIInfoAttribute GetUIInfo(Type screenType)
        {
            if (this.typeToUIInfo.TryGetValue(screenType, out var outUIInfo)) return outUIInfo;

            var uiInfo = screenType.GetCustomAttribute<UIInfoAttribute>();
            this.typeToUIInfo.Add(screenType, uiInfo);

            return uiInfo;
        }

#region Inject

        private readonly IObjectResolver   resolver;
        private readonly IGameAssetService gameAssetService;
        private readonly IMessageService   messageService;

#endregion

#region Cache

        private readonly Dictionary<uint, HashSet<IScreenPresenter>> orderLayerToScreenShow = new();
        private readonly Dictionary<string, IScreenPresenter>        cachedScreens          = new();
        private readonly Dictionary<Type, UIInfoAttribute>           typeToUIInfo           = new();

#endregion

#region Message

        private void OnOpenSingleScene(OpenSingleSceneMessage message)
        {
        }

        private void OnReleaseScene(ReleaseSceneMessage obj)
        {
        }

        private void OnScreenInitialize(ScreenInitializeMessage message)
        {
        }

        private void OnScreenOpened(ScreenOpenedMessage message)
        {
            if (message.ScreenPresenter == null) return;
            var uiInfo = this.GetUIInfo(message.ScreenPresenter.GetType());

            if (!this.orderLayerToScreenShow.TryGetValue(uiInfo.OrderLayer, out var listScreen))
            {
                this.orderLayerToScreenShow.Add(uiInfo.OrderLayer, new HashSet<IScreenPresenter> { message.ScreenPresenter });

                return;
            }

            listScreen.Add(message.ScreenPresenter);
        }

        private void OnScreenClosed(ScreenClosedMessage message)
        {
            if (message.ScreenPresenter == null) return;
            var uiInfo = this.GetUIInfo(message.ScreenPresenter.GetType());

            if (!this.orderLayerToScreenShow.TryGetValue(uiInfo.OrderLayer, out var screens)) return;

            if (!screens.Contains(message.ScreenPresenter)) return;

            message.ScreenPresenter.SetParent(this.RootUIView.CloseLayerTransform);
            screens.Remove(message.ScreenPresenter);
            this.CurrentScreen = screens.LastOrDefault();
            Debug.Log($"Current Screen: {this.CurrentScreen}");
        }

        private void OnScreenDestroyed(ScreenDestroyedMessage message)
        {
        }

#endregion

#region IScreenFlowService

        public RootUIView RootUIView { get; set; }

        public IScreenPresenter CurrentScreen { get; private set; }

        public async UniTask<TPresenter> InitScreenManually<TPresenter>() where TPresenter : IScreenPresenter
        {
            await UniTask.WaitUntil(() => this.RootUIView != null);
            var view      = this.RootUIView.GetComponentInChildren<IUIView>();
            var presenter = await this.GetOrAddScreen<TPresenter>(view);
            presenter.BindData();

            return presenter;
        }

        public async UniTask<TPresenter> InitScreenManually<TPresenter, TModel>(TModel model) where TPresenter : IScreenPresenter<TModel>
        {
            await UniTask.WaitUntil(() => this.RootUIView != null);
            var view      = this.RootUIView.GetComponentInChildren<IUIView>();
            var presenter = await this.GetOrAddScreen<TPresenter>(view);
            presenter.BindData(model);

            return presenter;
        }

        public async UniTask<TPresenter> OpenScreenAsync<TPresenter>() where TPresenter : IScreenPresenter
        {
            var presenter = await this.GetOrAddScreen<TPresenter>();
            presenter.BindData();

            return presenter;
        }

        public async UniTask<TPresenter> OpenScreenAsync<TPresenter, TModel>(TModel model) where TPresenter : IScreenPresenter<TModel>
        {
            var presenter = await this.GetOrAddScreen<TPresenter>();
            presenter.BindData(model);

            return presenter;
        }

        public UniTask CloseCurrentScreenAsync()
        {
            if (this.CurrentScreen != null) return this.CurrentScreen.CloseViewAsync();

            LoggerService.Warning("Don't has any screen to close!");

            return UniTask.CompletedTask;
        }

        public UniTask CloseAllScreenAsync()
        {
            var closeScreenTasks = new List<UniTask>();
            foreach (var (_, listScreen) in this.orderLayerToScreenShow)
            {
                closeScreenTasks.AddRange(Enumerable.Select(listScreen, screen => screen.CloseViewAsync()));
            }

            return UniTask.WhenAll(closeScreenTasks);
        }

        public UniTask DestroyCurrentScreenAsync()
        {
            if (this.CurrentScreen != null) return this.CurrentScreen.DestroyViewAsync();

            LoggerService.Warning("Don't has any screen to destroy!");

            return UniTask.CompletedTask;
        }

        public UniTask DestroyAllScreenAsync()
        {
            var screens = new List<IScreenPresenter>();
            foreach (var (_, listScreen) in this.orderLayerToScreenShow)
            {
                screens.AddRange(listScreen);
            }

            return UniTask.WhenAll(screens.Select(screen => screen.DestroyViewAsync()));
        }

#endregion
    }
}