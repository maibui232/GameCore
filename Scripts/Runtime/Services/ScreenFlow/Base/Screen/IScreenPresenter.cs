namespace GameCore.Services.ScreenFlow.Base.Screen
{
    using Cysharp.Threading.Tasks;
    using GameCore.Services.Message;
    using GameCore.Services.ScreenFlow.MVP;
    using UnityEngine;

    public interface IScreenPresenter : IUIPresenter
    {
        void SetView(IScreenView view);
    }

    public interface IScreenPresenter<TModel> : IScreenPresenter, IUIPresenter<TModel>
    {
    }

    public abstract class BaseScreenPresenter<TView> : IScreenPresenter where TView : BaseScreenView
    {
#region Inject

        protected readonly IMessageService MessageService;

#endregion

#region Cache

        private ScreenInitializeMessage screenInitializeMessage;
        private ScreenOpenedMessage     screenOpenedMessage;
        private ScreenClosedMessage     screenClosedMessage;
        private ScreenDestroyedMessage  screenDestroyedMessage;

#endregion

        protected BaseScreenPresenter(IMessageService messageService)
        {
            this.screenInitializeMessage = new ScreenInitializeMessage(this);
            this.screenOpenedMessage     = new ScreenOpenedMessage(this);
            this.screenClosedMessage     = new ScreenClosedMessage(this);
            this.screenDestroyedMessage  = new ScreenDestroyedMessage(this);

            this.MessageService = messageService;
        }

        protected TView View { get; private set; }

        public void SetParent(Transform parent)
        {
            if (this.View != null)
            {
                this.View.SetParent(parent);
            }
        }

        public void SetView(IScreenView view)
        {
            this.View = view as TView;
        }

        public async UniTask InitView()
        {
            this.MessageService.Publish(this.screenInitializeMessage);
            await this.OnViewInitAsync();
        }

        public ViewStatus ViewStatus { get; private set; }

        public abstract UniTask BindData();

        public async void OpenView()
        {
            await this.OpenViewAsync();
        }

        public async UniTask OpenViewAsync()
        {
            await UniTask.WhenAll(this.OnViewOpenAsync(), this.View.OpenViewAsync());
            this.ViewStatus = ViewStatus.Open;
            this.MessageService.Publish(this.screenOpenedMessage);
        }

        public void CloseView()
        {
            this.CloseViewAsync().Forget();
        }

        public async UniTask CloseViewAsync()
        {
            await UniTask.WhenAll(this.OnViewCloseAsync(), this.View.CloseViewAsync());
            this.ViewStatus = ViewStatus.Close;
            this.MessageService.Publish(this.screenClosedMessage);
        }

        public void DestroyView()
        {
            this.DestroyViewAsync().Forget();
        }

        public async UniTask DestroyViewAsync()
        {
            this.MessageService.Publish(this.screenDestroyedMessage);
            await this.OnViewDestroyAsync();
            this.View.DestroyView();
        }

        public void ShowView()
        {
            this.View.ShowView();
            this.ViewStatus = ViewStatus.Open;
        }

        public void HideView()
        {
            this.View.HideView();
            this.ViewStatus = ViewStatus.Hide;
        }

        public virtual void Dispose()
        {
        }

#region Callback Method

        protected virtual UniTask OnViewInitAsync()
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnViewOpenAsync()
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnViewCloseAsync()
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnViewDestroyAsync()
        {
            return UniTask.CompletedTask;
        }

#endregion
    }

    public abstract class BaseScreenPresenter<TView, TModel> : BaseScreenPresenter<TView>, IScreenPresenter<TModel> where TView : BaseScreenView
    {
        protected BaseScreenPresenter(IMessageService messageService) : base(messageService)
        {
        }

        public TModel Model { get; private set; }

        public sealed override UniTask BindData()
        {
            return UniTask.CompletedTask;
        }

        public abstract UniTask BindData(TModel model);
    }
}