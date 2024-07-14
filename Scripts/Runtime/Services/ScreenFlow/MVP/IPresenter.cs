namespace GameCore.Services.ScreenFlow.MVP
{
    using System;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public enum ViewStatus
    {
        Open,
        Close,
        Hide
    }

    public interface IPresenter
    {
    }

    public interface IUIPresenter : IDisposable
    {
        ViewStatus ViewStatus { get; }
        UniTask    BindData();
        void       SetParent(Transform parent);
        UniTask    InitView();
        void       OpenView();
        UniTask    OpenViewAsync();
        void       CloseView();
        UniTask    CloseViewAsync();
        void       DestroyView();
        UniTask    DestroyViewAsync();
        void       ShowView();
        void       HideView();
    }

    public interface IUIPresenter<TModel> : IUIPresenter
    {
        TModel  Model { get; }
        UniTask BindData(TModel model);
    }
}