namespace GameCore.Services.ScreenFlow.MVP
{
    using UnityEngine;

    public interface IView
    {
        GameObject ViewObject { get; }
        void       SetParent(Transform parent);
    }

    public interface IUIView : IView
    {
        void OpenView();
        void CloseView();
        void DestroyView();
    }
}