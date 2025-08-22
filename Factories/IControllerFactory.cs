using System.Threading.Tasks;
using UnityEngine;

namespace Code.MVC
{
    public interface IControllerFactory
    {
        // ============================
        // Synchronous methods
        // ============================
        TController Create<TController>()
            where TController : class, IController, new();

        TController Create<TController, TView, TModel>(TView view)
            where TController : Controller<TView, TModel>, new()
            where TView : Component, IView
            where TModel : class, IModel, new();

        TController Create<TController, TView, TModel, TData>(TView view, TData data)
            where TController : Controller<TView, TModel>, new()
            where TView : Component, IView
            where TModel : class, IModel, new();

        TController CreateWithModel<TController, TView, TModel>(TView view, TModel model)
            where TController : Controller<TView, TModel>, new()
            where TView : Component, IView
            where TModel : class, IModel, new();

        // ============================
        // Asynchronous methods
        // ============================
        Task<TController> CreateAsync<TController>()
            where TController : class, IController, new();

        Task<TController> CreateAsync<TController, TView, TModel>(TView view)
            where TController : Controller<TView, TModel>, new()
            where TView : Component, IView
            where TModel : class, IModel, new();

        Task<TController> CreateAsync<TController, TView, TModel, TData>(TView view, TData data)
            where TController : Controller<TView, TModel>, new()
            where TView : Component, IView
            where TModel : class, IModel, new();

        Task<TController> CreateWithModelAsync<TController, TView, TModel>(TView view, TModel model)
            where TController : Controller<TView, TModel>, new()
            where TView : Component, IView
            where TModel : class, IModel, new();
    }
}