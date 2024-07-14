namespace GameCore.Services.ScreenFlow.Base.Item
{
    using GameCore.Services.ScreenFlow.MVP;

    public interface IItemPresenter : IUIPresenter
    {
        void SetView(IItemView view);
    }

    public interface IItemPresenter<TModel> : IItemPresenter, IUIPresenter<TModel>
    {
    }
}