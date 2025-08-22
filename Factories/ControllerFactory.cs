using System.Threading.Tasks;
using UnityEngine;
#if ZENJECT
using Zenject;
#endif

namespace Code.MVC
{
    public class ControllerFactory : IControllerFactory
    {
        private readonly PlainControllerFactory _plainFactory = new PlainControllerFactory();

    #if ZENJECT
        private readonly ZenjectControllerFactory _zenFactory;
    #endif

#if ZENJECT
    public ControllerFactory(DiContainer container)
    {
        _zenFactory = new ZenjectControllerFactory(container);
    }
#else
        public ControllerFactory()
        {
            
        }
#endif

        // ============================
        // Sync: Create without data
        // ============================
        public TController Create<TController>()
            where TController : class, IController, new()
        {
    #if ZENJECT
            if (_zenFactory != null) return _zenFactory.Create<TController>();
    #endif
            return _plainFactory.Create<TController>();
        }

        // ============================
        // Sync: Create with view (no data)
        // ============================
        public TController Create<TController, TView, TModel>(TView view)
            where TController : Controller<TView, TModel>, new()
            where TView : Component, IView
            where TModel : class, IModel, new()
        {
    #if ZENJECT
            if (_zenFactory != null) return _zenFactory.Create<TController, TView, TModel>(view);
    #endif
            return _plainFactory.Create<TController, TView, TModel>(view);
        }

        // ============================
        // Sync: Create with view and data
        // ============================
        public TController Create<TController, TView, TModel, TData>(TView view, TData data)
            where TController : Controller<TView, TModel>, new()
            where TView : Component, IView
            where TModel : class, IModel, new()
        {
    #if ZENJECT
            if (_zenFactory != null) return _zenFactory.Create<TController, TView, TModel, TData>(view, data);
    #endif
            return _plainFactory.Create<TController, TView, TModel, TData>(view, data);
        }

        // ============================
        // Sync: Create with external model
        // ============================
        public TController CreateWithModel<TController, TView, TModel>(TView view, TModel model)
            where TController : Controller<TView, TModel>, new()
            where TView : Component, IView
            where TModel : class, IModel, new()
        {
    #if ZENJECT
            if (_zenFactory != null) return _zenFactory.CreateWithModel<TController, TView, TModel>(view, model);
    #endif
            return _plainFactory.CreateWithModel<TController, TView, TModel>(view, model);
        }

        // ============================
        // Async: Create without data
        // ============================
        public Task<TController> CreateAsync<TController>()
            where TController : class, IController, new()
        {
    #if ZENJECT
            if (_zenFactory != null) return _zenFactory.CreateAsync<TController>();
    #endif
            return _plainFactory.CreateAsync<TController>();
        }

        // ============================
        // Async: Create with view (no data)
        // ============================
        public Task<TController> CreateAsync<TController, TView, TModel>(TView view)
            where TController : Controller<TView, TModel>, new()
            where TView : Component, IView
            where TModel : class, IModel, new()
        {
    #if ZENJECT
            if (_zenFactory != null) return _zenFactory.CreateAsync<TController, TView, TModel>(view);
    #endif
            return _plainFactory.CreateAsync<TController, TView, TModel>(view);
        }

        // ============================
        // Async: Create with view and data
        // ============================
        public Task<TController> CreateAsync<TController, TView, TModel, TData>(TView view, TData data)
            where TController : Controller<TView, TModel>, new()
            where TView : Component, IView
            where TModel : class, IModel, new()
        {
    #if ZENJECT
            if (_zenFactory != null) return _zenFactory.CreateAsync<TController, TView, TModel, TData>(view, data);
    #endif
            return _plainFactory.CreateAsync<TController, TView, TModel, TData>(view, data);
        }

        // ============================
        // Async: Create with external model
        // ============================
        public Task<TController> CreateWithModelAsync<TController, TView, TModel>(TView view, TModel model)
            where TController : Controller<TView, TModel>, new()
            where TView : Component, IView
            where TModel : class, IModel, new()
        {
    #if ZENJECT
            if (_zenFactory != null) return _zenFactory.CreateWithModelAsync<TController, TView, TModel>(view, model);
    #endif
            return _plainFactory.CreateWithModelAsync<TController, TView, TModel>(view, model);
        }
    }

}