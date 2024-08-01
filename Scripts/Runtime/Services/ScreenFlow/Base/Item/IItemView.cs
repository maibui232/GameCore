namespace GameCore.Services.ScreenFlow.Base.Item
{
    using GameCore.Services.ScreenFlow.MVP;
    using UnityEngine;

    public interface IItemView : IUIView
    {
    }

    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BaseItemView : MonoBehaviour, IItemView
    {
#region Field

        [SerializeField] private CanvasGroup canvasGroup;

#endregion

#region MonoBehaviour

#if UNITY_EDITOR
        private void OnValidate()
        {
            this.canvasGroup ??= this.GetComponent<CanvasGroup>();
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

#region Implement IItemView

        public GameObject ViewObject { get; private set; }

        public void SetParent(Transform parent)
        {
            this.ViewObject.transform.SetParent(parent);
        }

        public void OpenView()
        {
            this.canvasGroup.alpha          = 1;
            this.canvasGroup.blocksRaycasts = true;
        }

        public void CloseView()
        {
            this.canvasGroup.blocksRaycasts = false;
            this.canvasGroup.alpha          = 0;
        }

        public void DestroyView()
        {
            Destroy(this.gameObject);
        }

#endregion
    }
}