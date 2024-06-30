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
        UniTask<TPresenter> OpenScreenAsync<TPresenter>(bool overlap = false) where TPresenter : IScreenPresenter;
        UniTask<TPresenter> OpenScreenAsync<TPresenter, TModel>(TModel model, bool overlap = false) where TPresenter : IScreenPresenter<TModel>;
        UniTask             CloseCurrentScreenAsync();
        UniTask             CloseAllScreenAsync();
        UniTask             DestroyCurrentScreenAsync();
        UniTask             DestroyAllScreenAsync();
    }

    public class ScreenFlowService : IScreenFlowService
    {
        #region Inject

        private readonly IObjectResolver   resolver;
        private readonly IGameAssetService gameAssetService;
        private readonly IMessageService   messageService;

        #endregion

        #region Cache

        private readonly Dictionary<uint, List<IScreenPresenter>> orderLayerToScreenShow = new();
        private readonly Dictionary<string, IScreenPresenter>     cachedScreens          = new();
        private readonly Dictionary<Type, UIInfoAttribute>        typeToUIInfo           = new();

        #endregion

        public ScreenFlowService
        (
            IObjectResolver resolver,
            IGameAssetService gameAssetService,
            IMessageService messageService
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

            if (!this.orderLayerToScreenShow.TryGetValue(uiInfo.OrderLayer, out var screens)) return;

            screens.Add(message.ScreenPresenter);
        }

        private void OnScreenClosed(ScreenClosedMessage message)
        {
            if (message.ScreenPresenter == null) return;
            var uiInfo = this.GetUIInfo(message.ScreenPresenter.GetType());

            if (!this.orderLayerToScreenShow.TryGetValue(uiInfo.OrderLayer, out var screens)) return;

            if (!screens.Contains(message.ScreenPresenter)) return;

            screens.Remove(message.ScreenPresenter);
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

        public async UniTask<TPresenter> OpenScreenAsync<TPresenter>(bool overlap = false) where TPresenter : IScreenPresenter
        {
            var presenter = await this.GetOrAddScreen<TPresenter>();
            presenter.BindData();
            return presenter;
        }

        public async UniTask<TPresenter> OpenScreenAsync<TPresenter, TModel>(TModel model, bool overlap = false) where TPresenter : IScreenPresenter<TModel>
        {
            var uiInfo = this.GetUIInfo<TPresenter>();
            if (this.orderLayerToScreenShow.TryGetValue(uiInfo.OrderLayer, out var screenPresenters))
            {
                var closeTasks = new List<UniTask>();
                screenPresenters.ForEach(screen => closeTasks.Add(screen.CloseViewAsync()));
                await UniTask.WhenAll(closeTasks);
            }

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
            var destroyViewTasks = new List<UniTask>();
            foreach (var (_, listScreen) in this.orderLayerToScreenShow)
            {
                destroyViewTasks.AddRange(Enumerable.Select(listScreen, screen => screen.DestroyViewAsync()));
            }

            return UniTask.WhenAll(destroyViewTasks);
        }

        #endregion

        private async UniTask<TPresenter> GetOrAddScreen<TPresenter>(IUIView viewParam = null) where TPresenter : IScreenPresenter
        {
            var uiInfo = this.GetUIInfo<TPresenter>();
            if (uiInfo == null)
            {
                throw new Exception($"Could not find screen with addressable id: {nameof(UIInfoAttribute)}");
            }

            var hasCachedPresenter = this.cachedScreens.TryGetValue(uiInfo.AddressableId, out var outPresenter);
            var presenter = hasCachedPresenter
                ? (TPresenter)outPresenter
                : (TPresenter)VContainerExtensions.ContainerBuilder.Register<TPresenter>(Lifetime.Singleton).As().Build().SpawnInstance(this.resolver);

            if (!hasCachedPresenter)
            {
                this.cachedScreens.Add(uiInfo.AddressableId, presenter);
                var     prefab = await this.gameAssetService.LoadAssetAsync<GameObject>(uiInfo.AddressableId);
                IUIView view;
                if (viewParam == null)
                {
                    var parentTransform = this.RootUIView.GetOrCreateOverlayCanvas((int)uiInfo.OrderLayer).transform;
                    view = this.resolver.Instantiate(prefab, parentTransform).GetComponent<IUIView>();
                }
                else
                {
                    view = viewParam;
                }

                presenter.SetView(view);
                presenter.InitView();
            }

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
            if (this.typeToUIInfo.TryGetValue(screenType, out var outUIInfo))
            {
                return outUIInfo;
            }

            var uiInfo = screenType.GetCustomAttribute<UIInfoAttribute>();
            this.typeToUIInfo.Add(screenType, uiInfo);
            return uiInfo;
        }
    }
}