using System;

namespace Code.MVC
{
    public class Controller<TView, TModel> : IController
        where TView  : UnityEngine.Component, IView
        where TModel : class, IModel, new()
    {
        protected TView View  { get; private set; }
        protected TModel Model { get; private set; }

        public bool IsInitialized { get; private set; }
        public bool IsClosed { get; private set; }

        private bool _viewTerminatedExternally;

        protected Controller()
        {
            Model = new TModel();
        }

        /// <summary> Allows setting an external model before binding the View. </summary>
        public void SetModel(TModel model)
        {
            if (View != null)
                throw new InvalidOperationException("Model can be set only before ApplyView().");
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;
            OnInitialize();
        }

        public virtual void Initialize<TData>(TData data)
        {
            if (IsInitialized) return;
            IsInitialized = true;
            OnInitialize(data);
        }

        public void ApplyView(IView view)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));
            if (IsClosed) throw new InvalidOperationException("Controller is already closed.");

            var casted = view as TView;
            if (casted == null)
                throw new ArgumentException($"View must be of type {typeof(TView).Name}", nameof(view));

            DetachViewEvents();

            View = casted;
            View.SetController(this);
            View.ApplyModel(Model);

            AttachViewEvents();

            OnApplyView(View);
            Subscribe();
        }

        public void Close()
        {
            if (IsClosed) return;
            IsClosed = true;

            try
            {
                Unsubscribe();
            }
            catch { /* swallow */ }

            if (View != null)
            {
                OnCloseView(View);
                DetachViewEvents();

                var v = View;
                View = null;

                if (!_viewTerminatedExternally)
                {
                    v.Close();
                }
            }
        }

        protected virtual void OnInitialize() {}
        protected virtual void OnInitialize<TData>(TData data) {}

        protected virtual void OnApplyView(TView view) {}
        protected virtual void OnCloseView(TView view) {}

        /// <summary> Subscriptions to model/view/services events. </summary>
        protected virtual void Subscribe() {}

        /// <summary> Unsubscriptions from model/view/services events. </summary>
        protected virtual void Unsubscribe() {}

        private void AttachViewEvents()
        {
            if (View == null) return;
            View.Closed    += OnViewClosed;
            View.Destroyed += OnViewDestroyed;
            _viewTerminatedExternally = false;
        }

        private void DetachViewEvents()
        {
            if (View == null) return;
            View.Closed    -= OnViewClosed;
            View.Destroyed -= OnViewDestroyed;
        }

        private void OnViewClosed()
        {
            Close();
        }

        private void OnViewDestroyed()
        {
            _viewTerminatedExternally = true;
            Close();
        }
    }
}