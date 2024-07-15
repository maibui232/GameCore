namespace GameCore.Services.ScreenFlow.Base.Item
{
    using Cysharp.Threading.Tasks;
    using GameCore.Services.ScreenFlow.MVP;
    using UnityEngine;

    public interface IItemPresenter : IUIPresenter
    {
        void SetView(IItemView view);
    }

    public interface IItemPresenter<TModel> : IItemPresenter, IUIPresenter<TModel>
    {
    }

    public abstract class BaseItemPresenter<TView> : IItemPresenter where TView : BaseItemView
    {
        protected TView View { get; private set; }

#region Implement IItemPresenter

        public ViewStatus ViewStatus { get; private set; }

        public abstract UniTask BindData();

        public void SetView(IItemView view)
        {
            this.View = view as TView;
        }

        public virtual void Dispose()
        {
        }

        public void SetParent(Transform parent)
        {
            this.View.SetParent(parent);
        }

        public UniTask InitView()
        {
            return this.OnViewInitAsync();
        }

        public void OpenView()
        {
            this.OpenViewAsync().Forget();
        }

        public async UniTask OpenViewAsync()
        {
            await UniTask.WhenAll(this.OnViewOpenAsync(), this.View.OpenViewAsync());
            this.ViewStatus = ViewStatus.Open;
        }

        public void CloseView()
        {
            this.CloseViewAsync().Forget();
        }

        public async UniTask CloseViewAsync()
        {
            await UniTask.WhenAll(this.OnViewCloseAsync(), this.View.CloseViewAsync());
            this.ViewStatus = ViewStatus.Close;
        }

        public void DestroyView()
        {
            this.DestroyViewAsync().Forget();
        }

        public async UniTask DestroyViewAsync()
        {
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

#endregion

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

    public abstract class BaseItemPresenter<TView, TModel> : BaseItemPresenter<TView>, IItemPresenter<TModel> where TView : BaseItemView
    {
        public TModel Model { get; private set; }

        public sealed override UniTask BindData()
        {
            return UniTask.CompletedTask;
        }

        public abstract UniTask BindData(TModel model);
    }
}