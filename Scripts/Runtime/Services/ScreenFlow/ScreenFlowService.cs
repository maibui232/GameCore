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
    using GameCore.Services.ScreenFlow.Base.Screen;
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

        private void HideAllScreenIfOverlap(ScreenInfoAttribute screenInfo)
        {
            if (screenInfo.Overlap) return;
            if (!this.orderLayerToScreenShow.TryGetValue(screenInfo.OrderLayer, out var screenPresenters)) return;

            foreach (var screen in screenPresenters)
            {
                screen.HideView();
            }
        }

        private async UniTask<TPresenter> GetOrAddScreen<TPresenter>(IScreenView viewParam = null) where TPresenter : IScreenPresenter
        {
            var uiInfo = this.GetUIInfo<TPresenter>();

            if (uiInfo == null) throw new Exception($"Could not find screen with addressable id: {nameof(ScreenInfoAttribute)}");

            this.HideAllScreenIfOverlap(uiInfo);

            var hasCachedPresenter = this.cachedScreens.TryGetValue(uiInfo.AddressableId, out var outPresenter);
            var presenter = hasCachedPresenter
                                ? (TPresenter)outPresenter
                                : this.resolver.InstantiateConcrete<TPresenter>();

            var parent = this.RootUIView.GetOrCreateOverlayCanvas(uiInfo.OrderLayer).transform;
            if (!hasCachedPresenter)
            {
                this.cachedScreens.Add(uiInfo.AddressableId, presenter);
                var view = viewParam;
                if (view == null)
                {
                    var prefab = await this.gameAssetService.LoadAssetAsync<GameObject>(uiInfo.AddressableId);
                    view = this.resolver.Instantiate(prefab, parent).GetComponent<IScreenView>();
                }

                presenter.SetView(view);
                presenter.InitView();
            }

            presenter.SetParent(parent);
            this.CurrentScreen = presenter;
            await presenter.OpenViewAsync();

            return presenter;
        }

        private ScreenInfoAttribute GetUIInfo<TPresenter>() where TPresenter : IScreenPresenter
        {
            return this.GetUIInfo(typeof(TPresenter));
        }

        private ScreenInfoAttribute GetUIInfo(Type screenType)
        {
            if (this.typeToUIInfo.TryGetValue(screenType, out var outUIInfo)) return outUIInfo;

            var uiInfo = screenType.GetCustomAttribute<ScreenInfoAttribute>();
            this.typeToUIInfo.Add(screenType, uiInfo);

            return uiInfo;
        }

#region Inject

        private readonly IObjectResolver   resolver;
        private readonly IGameAssetService gameAssetService;
        private readonly IMessageService   messageService;

#endregion

#region Cache

        private readonly Dictionary<OrderLayer, HashSet<IScreenPresenter>> orderLayerToScreenShow = new();
        private readonly Dictionary<string, IScreenPresenter>              cachedScreens          = new();
        private readonly Dictionary<Type, ScreenInfoAttribute>             typeToUIInfo           = new();

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
            var lastScreenShow = screens.LastOrDefault();
            lastScreenShow?.ShowView();
            this.CurrentScreen = lastScreenShow;
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
            var view      = this.RootUIView.GetComponentInChildren<IScreenView>();
            var presenter = await this.GetOrAddScreen<TPresenter>(view);
            await presenter.BindData();

            return presenter;
        }

        public async UniTask<TPresenter> InitScreenManually<TPresenter, TModel>(TModel model) where TPresenter : IScreenPresenter<TModel>
        {
            await UniTask.WaitUntil(() => this.RootUIView != null);
            var view      = this.RootUIView.GetComponentInChildren<IScreenView>();
            var presenter = await this.GetOrAddScreen<TPresenter>(view);
            await presenter.BindData(model);

            return presenter;
        }

        public async UniTask<TPresenter> OpenScreenAsync<TPresenter>() where TPresenter : IScreenPresenter
        {
            var presenter = await this.GetOrAddScreen<TPresenter>();
            await presenter.BindData();

            return presenter;
        }

        public async UniTask<TPresenter> OpenScreenAsync<TPresenter, TModel>(TModel model) where TPresenter : IScreenPresenter<TModel>
        {
            var presenter = await this.GetOrAddScreen<TPresenter>();
            await presenter.BindData(model);

            return presenter;
        }

        public UniTask CloseCurrentScreenAsync()
        {
            if (this.CurrentScreen != null) return this.CurrentScreen.CloseViewAsync();

            LoggerUtils.Warning("Don't has any screen to close!");

            return UniTask.CompletedTask;
        }

        public UniTask CloseAllScreenAsync()
        {
            var listScreen = new List<IScreenPresenter>();
            foreach (var (_, screens) in this.orderLayerToScreenShow)
            {
                listScreen = screens.ToList();
            }

            return UniTask.WhenAll(listScreen.Select(item => item.CloseViewAsync()));
        }

        public UniTask DestroyCurrentScreenAsync()
        {
            if (this.CurrentScreen != null) return this.CurrentScreen.DestroyViewAsync();

            LoggerUtils.Warning("Don't has any screen to destroy!");

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