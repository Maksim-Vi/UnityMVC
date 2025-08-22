using System.Threading.Tasks;
using UnityEngine;

namespace Code.MVC
{
    public abstract class ControllerFactoryBase : IControllerFactory
    {
        public abstract TController Create<TController>()
            where TController : class, IController, new();

        public abstract TController Create<TController, TView, TModel>(TView view)
            where TController : Controller<TView, TModel>, new()
            where TView : UnityEngine.Component, IView
            where TModel : class, IModel, new();

        public abstract TController Create<TController, TView, TModel, TData>(TView view, TData data)
            where TController : Controller<TView, TModel>, new()
            where TView : UnityEngine.Component, IView
            where TModel : class, IModel, new();

        public abstract TController CreateWithModel<TController, TView, TModel>(TView view, TModel model)
            where TController : Controller<TView, TModel>, new()
            where TView : UnityEngine.Component, IView
            where TModel : class, IModel, new();
        
        // ============================
        // Async (default impl wraps sync)
        // ============================
        public virtual Task<TController> CreateAsync<TController>()
            where TController : class, IController, new()
            => Task.FromResult(Create<TController>());

        public virtual Task<TController> CreateAsync<TController, TView, TModel>(TView view)
            where TController : Controller<TView, TModel>, new()
            where TView : UnityEngine.Component, IView
            where TModel : class, IModel, new()
            => Task.FromResult(Create<TController, TView, TModel>(view));

        public virtual Task<TController> CreateAsync<TController, TView, TModel, TData>(TView view, TData data)
            where TController : Controller<TView, TModel>, new()
            where TView : UnityEngine.Component, IView
            where TModel : class, IModel, new()
            => Task.FromResult(Create<TController, TView, TModel, TData>(view, data));

        public virtual Task<TController> CreateWithModelAsync<TController, TView, TModel>(TView view, TModel model)
            where TController : Controller<TView, TModel>, new()
            where TView : UnityEngine.Component, IView
            where TModel : class, IModel, new()
            => Task.FromResult(CreateWithModel<TController, TView, TModel>(view, model));
    }
}