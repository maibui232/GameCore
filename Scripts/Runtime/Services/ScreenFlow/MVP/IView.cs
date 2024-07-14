namespace GameCore.Services.ScreenFlow.MVP
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public interface IView
    {
        GameObject ViewObject { get; }
        void       SetParent(Transform parent);
    }

    public interface IUIView : IView
    {
        void    ShowView();
        void    HideView();
        void    OpenView();
        UniTask OpenViewAsync();
        void    CloseView();
        UniTask CloseViewAsync();
        void    DestroyView();
    }
}