namespace GameCore.Services.ScreenFlow.Base
{
    using GameCore.Services.ScreenFlow.MVP;
    using GameCore.Utils.UIElement;
    using UnityEngine;

    public interface IUIView : IView
    {
        void ShowView();
        void HideView();
        void OpenView();
        void CloseView();
        void DestroyView();
    }

    [RequireComponent(typeof(CanvasGroup)), RequireComponent(typeof(UITransition))]
    public abstract class BaseUIView : MonoBehaviour, IUIView
    {
        #region Field

        [SerializeField] private UITransition uiTransition;
        [SerializeField] private CanvasGroup  canvasGroup;

        #endregion

        #region MonoBehaviour

#if UNITY_EDITOR
        private void OnValidate()
        {
            this.uiTransition ??= this.GetComponent<UITransition>();
            this.canvasGroup  ??= this.GetComponent<CanvasGroup>();
        }
#endif

        private void Awake()
        {
            this.ViewObject = this.gameObject;
            this.AwakeEvent();
        }

        protected virtual void AwakeEvent()
        {
        }

        private void OnEnable()
        {
            this.OnEnableEvent();
        }

        protected virtual void OnEnableEvent()
        {
        }

        private void OnDisable()
        {
            this.OnDisableEvent();
        }

        protected virtual void OnDisableEvent()
        {
        }

        private void OnDestroy()
        {
            this.OnDestroyEvent();
        }

        protected virtual void OnDestroyEvent()
        {
        }

        #endregion

        #region Implement IUIView

        public GameObject ViewObject { get; private set; }

        public void SetParent(Transform parent)
        {
            this.ViewObject.transform.SetParent(parent);
        }

        public void ShowView()
        {
            this.canvasGroup.alpha = 1;
        }

        public void HideView()
        {
            this.canvasGroup.alpha = 0;
        }

        public void OpenView()
        {
            this.canvasGroup.alpha = 1;
        }

        public void CloseView()
        {
            this.canvasGroup.alpha = 0;
        }

        public void DestroyView()
        {
            Destroy(this.gameObject);
        }

        #endregion
    }
}