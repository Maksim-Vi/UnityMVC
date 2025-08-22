============ Zenject ===================
1) install ControllerFactory in MAIN Installer
public class CoreInstaller : MonoInstaller
    {              
        public override void InstallBindings()
        { 
            Container.Bind<ControllerFactory>().AsSingle();
        }
    }
}

2) install HelmetFactory in Installer
public class GameplayInstaller : MonoInstaller
    {              
        public override void InstallBindings()
        { 
            Container.BindInterfacesAndSelfTo<HelmetFactory>().AsSingle(); 
        }
    }
}

3) create Model, View, Controller, Factory, Manager
public class HelmetModel : IModel
{
    public string Name { get; set; }
}

public class HelmetView : View<HelmetModel>
{
    [SerializeField] private string helmetName;

    public string HelmetName => helmetName;

    public void Display()
    {
        Debug.Log($"[HelmetView] Displaying helmet: {helmetName}");
    }
}

public class HelmetController : Controller<HelmetView, HelmetModel>
{
    public override void Initialize()
    {
        Debug.Log($"[HelmetController] Initialized with model: {Model.Name} (DEF {Model.Defense})");
    }

    public override void ApplyView(HelmetView view)
    {
        View = view;
        View.Display();
    }

    public override void Initialize(HelmetModel data)
    {
        Model = data;
        Initialize();
    }
}

public class HelmetFactory
{
    private readonly ControllerFactory _controllerFactory;

    public HelmetFactory(ControllerFactory controllerFactory)
    {
        _controllerFactory = controllerFactory;
    }

    public HelmetController Create(HelmetView view, HelmetModel model)
    {
        return _controllerFactory.CreateWithModel<HelmetController, HelmetView, HelmetModel>(view, model);
    }

    public Task<HelmetController> CreateAsync(HelmetView view, HelmetModel model)
    {
        return _controllerFactory.CreateWithModelAsync<HelmetController, HelmetView, HelmetModel>(view, model);
    }
}

public class HelmetManager : IInitializable
{
    private readonly HelmetFactory _helmetFactory;

    public GameManager(HelmetFactory helmetFactory)
    {
        _helmetFactory = helmetFactory;
    }

    public void Initialize()
    {
        var helmetModel = new HelmetModel("Iron Helmet", 25);
        var helmetView = GameObject.FindObjectOfType<HelmetView>();
        var helmetController = _helmetFactory.Create(helmetView, helmetModel);
    }

    private async Task InitAsync(HelmetView view, HelmetModel model)
    {
        var controller = await _helmetFactory.CreateAsync(view, model);
    }
}

=========================================

============ WITHOUT Zenject ============
1) create ControllerFactory, HelmetFactory
public class GameBase : MonoBehaviour
{
    private HelmetManager _helmetManager;

    private void Awake()
    {
        var controllerFactory = new ControllerFactory();
        var helmetFactory = new HelmetFactory(controllerFactory);

        _helmetManager = new HelmetManager(helmetFactory);
    }

    private void Start()
    {
        _helmetManager.Init();
    }
}

3) create Model, View, Controller, Factory, Manager
public class HelmetModel : IModel
{
    public string Name { get; set; }
}

public class HelmetView : View<HelmetModel>
{
    [SerializeField] private string helmetName;

    public string HelmetName => helmetName;

    public void Display()
    {
        Debug.Log($"[HelmetView] Displaying helmet: {helmetName}");
    }
}

public class HelmetController : Controller<HelmetView, HelmetModel>
{
    public override void Initialize()
    {
        Debug.Log($"[HelmetController] Initialized with model: {Model.Name} (DEF {Model.Defense})");
    }

    public override void ApplyView(HelmetView view)
    {
        View = view;
        View.Display();
    }

    public override void Initialize(HelmetModel data)
    {
        Model = data;
        Initialize();
    }
}

public class HelmetFactory
{
    private readonly ControllerFactory _controllerFactory;

    public HelmetFactory(ControllerFactory controllerFactory)
    {
        _controllerFactory = controllerFactory;
    }

    public HelmetController Create(HelmetView view, HelmetModel model)
    {
        return _controllerFactory.CreateWithModel<HelmetController, HelmetView, HelmetModel>(view, model);
    }

    public Task<HelmetController> CreateAsync(HelmetView view, HelmetModel model)
    {
        return _controllerFactory.CreateWithModelAsync<HelmetController, HelmetView, HelmetModel>(view, model);
    }
}

public class HelmetManager
{
    private readonly HelmetFactory _helmetFactory;

    public HelmetManager(HelmetFactory helmetFactory)
    {
        _helmetFactory = helmetFactory;
    }

    public void Init()
    {
        var helmetModel = new HelmetModel("Iron Helmet", 25);
        var helmetView = GameObject.FindObjectOfType<HelmetView>();

        var helmetController = _helmetFactory.Create(helmetView, helmetModel);
    }

    public async Task InitAsync()
    {
        var helmetModel = new HelmetModel("Iron Helmet", 25);
        var helmetView = GameObject.FindObjectOfType<HelmetView>();

        var controller = await _helmetFactory.CreateAsync(helmetView, helmetModel);
    }
}

=========================================

============ Extra info ===================

ControllerFactory has:

1) INPORTENT need to set Settings (top folders MVC/Settings) set or leave default path folder settings, set Zenject if use, click apply

== Sync ==

1) def controller
var controller = controllerFactory.Create<MyController>();

2) controller with View
var controller = controllerFactory.Create<MyController, MyView, MyModel>(myView);

3) controller with View + Data
var controller = controllerFactory.Create<MyController, MyView, MyModel, MyData>(myView, myData);

4) controller with View + Model
var controller = controllerFactory.CreateWithModel<MyController, MyView, MyModel>(myView, myModel);

== Async ==

1) def controller
var controller = await controllerFactory.CreateAsync<MyController>();

2) controller with View
var controller = await controllerFactory.CreateAsync<MyController, MyView, MyModel>(myView);

3) controller with View + Data
var controller = await controllerFactory.CreateAsync<MyController, MyView, MyModel, MyData>(myView, myData);

4) controller with View + Model
var controller = await controllerFactory.CreateWithModelAsync<MyController, MyView, MyModel>(myView, myModel);

=========================================
