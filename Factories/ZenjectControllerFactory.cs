#if MVC_USE_ZENJECT
using System.Threading.Tasks;
using Zenject;

namespace Code.MVC
{
    public class ZenjectControllerFactory : ControllerFactoryBase
    {
        private readonly DiContainer _container;

        public ZenjectControllerFactory(DiContainer container)
        {
            _container = container;
        }

        public override TController Create<TController>()
        {
            var controller = new TController();
            _container.Inject(controller);
            return controller;
        }

        public override TController Create<TController, TView, TModel>(TView view)
        {
            var controller = new TController();
            _container.Inject(controller);

            controller.Initialize();
            controller.ApplyView(view);
            return controller;
        }

        public override TController Create<TController, TView, TModel, TData>(TView view, TData data)
        {
            var controller = new TController();
            _container.Inject(controller);

            controller.Initialize(data);
            controller.ApplyView(view);
            return controller;
        }

        public override TController CreateWithModel<TController, TView, TModel>(TView view, TModel model)
        {
            var controller = new TController();
            _container.Inject(controller);

            controller.SetModel(model);
            controller.Initialize();
            controller.ApplyView(view);
            return controller;
        }

        // ============================
        // Async (if you want real async: Task.Run)
        // ============================
        public override Task<TController> CreateAsync<TController>()
            => Task.FromResult(Create<TController>());

        public override Task<TController> CreateAsync<TController, TView, TModel>(TView view)
            => Task.FromResult(Create<TController, TView, TModel>(view));

        public override Task<TController> CreateAsync<TController, TView, TModel, TData>(TView view, TData data)
            => Task.FromResult(Create<TController, TView, TModel, TData>(view, data));

        public override Task<TController> CreateWithModelAsync<TController, TView, TModel>(TView view, TModel model)
            => Task.FromResult(CreateWithModel<TController, TView, TModel>(view, model));
    }
}
#endif
