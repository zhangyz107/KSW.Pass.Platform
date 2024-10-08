using KSW.Reflections;

namespace KSW.Infrastructure;

/// <summary>
/// 服务上下文
/// </summary>
public class ServiceContext {
    /// <summary>
    /// 初始化服务上下文
    /// </summary>
    /// <param name="containerRegistry">容器</param>
    /// <param name="assemblyFinder">程序集查找器</param>
    /// <param name="typeFinder">类型查找器</param>
    public ServiceContext(IContainerRegistry containerRegistry, IAssemblyFinder assemblyFinder, ITypeFinder typeFinder ) {
        ContainerRegistry = containerRegistry ?? throw new ArgumentNullException( nameof( containerRegistry ) );
        AssemblyFinder = assemblyFinder ?? throw new ArgumentNullException( nameof( assemblyFinder ) );
        TypeFinder = typeFinder ?? throw new ArgumentNullException( nameof( typeFinder ) );
    }

    /// <summary>
    /// 主机生成器
    /// </summary>
    public IContainerRegistry ContainerRegistry { get; }

    /// <summary>
    /// 程序集查找器
    /// </summary>
    public IAssemblyFinder AssemblyFinder { get; }

    /// <summary>
    /// 类型查找器
    /// </summary>
    public ITypeFinder TypeFinder { get; }
}