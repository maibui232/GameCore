namespace GameCore.Services.ScreenFlow.Base
{
    using System;
    using Cysharp.Threading.Tasks;

    public interface IScreenPresenter : IDisposable
    {
        ScreenStatus ScreenStatus { get; }
        UniTask      BindData();
        void         SetView(IUIView view);
        UniTask      InitView();
        void         OpenView();
        UniTask      OpenViewAsync();
        void         CloseView();
        UniTask      CloseViewAsync();
        void         DestroyView();
        UniTask      DestroyViewAsync();
    }

    public interface IScreenPresenter<TModel> : IScreenPresenter
    {
        TModel  Model { get; }
        UniTask BindData(TModel model);
    }

    public enum ScreenStatus
    {
        Open,
        Close
    }
}