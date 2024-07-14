namespace GameCore.Services.ScreenFlow.Base.Item
{
    using System;
    using Cysharp.Threading.Tasks;
    using GameCore.Services.ScreenFlow.MVP;
    using UnityEngine;

    public abstract class BaseItemView : MonoBehaviour, IItemView
    {
#region Unity Event

        private void Awake()
        {
            this.ViewObject = this.gameObject;
        }

#endregion

#region Implement IItemView

        public GameObject ViewObject { get; private set; }

        public void SetParent(Transform parent)
        {
            this.ViewObject.transform.SetParent(parent);
        }

#endregion

        public void ShowView()
        {
            throw new NotImplementedException();
        }

        public void HideView()
        {
            throw new NotImplementedException();
        }

        public void OpenView()
        {
            throw new NotImplementedException();
        }

        public UniTask OpenViewAsync()
        {
            throw new NotImplementedException();
        }

        public void CloseView()
        {
            throw new NotImplementedException();
        }

        public UniTask CloseViewAsync()
        {
            throw new NotImplementedException();
        }

        public void DestroyView()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class BaseItemPresenter<TView> : IItemPresenter where TView : MonoBehaviour, IItemView
    {
        protected TView View { get; set; }
        
        public ViewStatus ViewStatus { get; private set; }

        public void SetView(IItemView view)
        {
            this.View = view as TView;
        }

        public abstract UniTask BindData();

        public virtual void Dispose()
        {
        }

        public void SetParent(Transform parent)
        {
            this.View.SetParent(parent);
        }

        public UniTask InitView()
        {
            return UniTask.CompletedTask;
        }

        public void OpenView()
        {
        }

        public UniTask OpenViewAsync()
        {
            return UniTask.CompletedTask;
        }

        public void CloseView()
        {
        }

        public UniTask CloseViewAsync()
        {
            return UniTask.CompletedTask;
        }

        public void DestroyView()
        {
        }

        public UniTask DestroyViewAsync()
        {
            return UniTask.CompletedTask;
        }

        public void ShowView()
        {
        }

        public void HideView()
        {
        }
    }
}