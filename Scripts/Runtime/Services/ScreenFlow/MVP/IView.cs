namespace GameCore.Services.ScreenFlow.MVP
{
    using Cysharp.Threading.Tasks;
    using GameCore.Services.ScreenFlow.UIElement;
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

    [RequireComponent(typeof(CanvasGroup), typeof(UITransition))]
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
            this.canvasGroup.alpha          = 1;
            this.canvasGroup.blocksRaycasts = true;
        }

        public void HideView()
        {
            this.canvasGroup.blocksRaycasts = false;
            this.canvasGroup.alpha          = 0;
        }

        public void OpenView()
        {
            this.OpenViewAsync().Forget();
        }

        public async UniTask OpenViewAsync()
        {
            this.canvasGroup.alpha = 1;
            await this.uiTransition.PlayIntro();
            this.canvasGroup.blocksRaycasts = true;
        }

        public void CloseView()
        {
            this.CloseViewAsync().Forget();
        }

        public async UniTask CloseViewAsync()
        {
            this.canvasGroup.blocksRaycasts = false;
            await this.uiTransition.PlayOutro();
            this.canvasGroup.alpha = 0;
        }

        public void DestroyView()
        {
            Destroy(this.gameObject);
        }

#endregion
    }
}