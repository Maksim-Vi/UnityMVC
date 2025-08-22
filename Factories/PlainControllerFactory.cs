using System.Threading.Tasks;

namespace Code.MVC
{
    public class PlainControllerFactory : ControllerFactoryBase
    {
        public override TController Create<TController>()
        {
            return new TController();
        }

        public override TController Create<TController, TView, TModel>(TView view)
        {
            var controller = new TController();
            controller.Initialize();
            controller.ApplyView(view);
            return controller;
        }

        public override TController Create<TController, TView, TModel, TData>(TView view, TData data)
        {
            var controller = new TController();
            controller.Initialize(data);
            controller.ApplyView(view);
            return controller;
        }

        public override TController CreateWithModel<TController, TView, TModel>(TView view, TModel model)
        {
            var controller = new TController();
            controller.SetModel(model);
            controller.Initialize();
            controller.ApplyView(view);
            return controller;
        }
        
        // Async
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