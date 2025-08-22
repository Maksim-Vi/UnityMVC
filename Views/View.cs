using System;
using UnityEngine;

namespace Code.MVC
{
    
    public class View<TModel> : MonoBehaviour, IView where TModel : class, IModel
    {
        protected TModel Model { get; private set; }

        public event Action Closed;
        public event Action Destroyed;

        private IController _controller;
        private bool _isClosed;
        private bool _destroyedRaised;

        private void Start()
        {
            OnStartEvent();
        }

        private void OnDestroy()
        {
            if (!_destroyedRaised)
            {
                _destroyedRaised = true;
                Destroyed?.Invoke();
            }
            OnDestroyEvent();
        }

        public void ApplyModel(IModel model)
        {
            Model = model as TModel;
            OnApplyModel(Model);
        }

        public void SetController(IController controller)
        {
            _controller = controller;
        }

        public virtual void Close()
        {
            if (_isClosed) return;
            _isClosed = true;

            Closed?.Invoke();
            Destroy(gameObject);
        }

        /// <summary> Apply Model. </summary>
        protected virtual void OnApplyModel(TModel model) {}

        /// <summary> Unity Start hook. </summary>
        protected virtual void OnStartEvent() {}

        /// <summary> Unity OnDestroy hook. </summary>
        protected virtual void OnDestroyEvent() {}
    }
}