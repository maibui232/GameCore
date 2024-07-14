namespace GameCore.Services.ScreenFlow.Base.Screen
{
    using GameCore.Services.ScreenFlow.MVP;

    public interface IScreenPresenter : IUIPresenter
    {
        void SetView(IScreenView view);
    }

    public interface IScreenPresenter<TModel> : IScreenPresenter, IUIPresenter<TModel>
    {
    }
}