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
        void       OpenView();
        void       CloseView();
        void       DestroyView();
    }

    public interface IUIPresenter<TModel> : IUIPresenter
    {
        TModel  Model { get; }
        UniTask BindData(TModel model);
    }
}