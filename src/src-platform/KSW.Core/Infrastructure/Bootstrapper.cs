using KSW.Helpers;
using KSW.Reflections;
using Prism.Ioc;

namespace KSW.Infrastructure; 

/// <summary>
/// 启动器
/// </summary>
public class Bootstrapper {
    /// <summary>
    /// 主机生成器
    /// </summary>
    private readonly IContainerRegistry _containerRegistry;
    /// <summary>
    /// 程序集查找器
    /// </summary>
    private readonly IAssemblyFinder _assemblyFinder;
    /// <summary>
    /// 类型查找器
    /// </summary>
    private readonly ITypeFinder _typeFinder;
    /// <summary>
    /// 服务配置操作列表
    /// </summary>
    private readonly List<Action> _serviceActions;

    /// <summary>
    /// 初始化启动器
    /// </summary>
    /// <param name="containerRegistry">主机生成器</param>
    public Bootstrapper(IContainerRegistry containerRegistry ) {
        _containerRegistry = containerRegistry ?? throw new ArgumentNullException( nameof( containerRegistry ) );
        _assemblyFinder = new AppDomainAssemblyFinder { AssemblySkipPattern = BootstrapperConfig.AssemblySkipPattern };
        _typeFinder = new AppDomainTypeFinder( _assemblyFinder );
        _serviceActions = new List<Action>();
    }

    /// <summary>
    /// 启动
    /// </summary>
    public virtual void Start() {
        ConfigureServices();
        ResolveServiceRegistrar();
        ExecuteServiceActions();
    }

    /// <summary>
    /// 配置服务
    /// </summary>
    protected virtual void ConfigureServices() {
        _containerRegistry.TryRegisterInstance(_assemblyFinder);
        _containerRegistry.TryRegisterInstance(_typeFinder);
    }

    /// <summary>
    /// 解析服务注册器
    /// </summary>
    protected virtual void ResolveServiceRegistrar() {
        var types = _typeFinder.Find<IServiceRegistrar>();
        var instances = types.Select( type => Reflection.CreateInstance<IServiceRegistrar>( type ) ).Where( t => t.Enabled ).OrderBy( t => t.OrderId ).ToList();
        var context = new ServiceContext(_containerRegistry, _assemblyFinder, _typeFinder );
        instances.ForEach( t => _serviceActions.Add( t.Register( context ) ) );
    }

    /// <summary>
    /// 执行延迟服务注册操作
    /// </summary>
    protected virtual void ExecuteServiceActions() {
        _serviceActions.ForEach( action => action?.Invoke() );
    }
}