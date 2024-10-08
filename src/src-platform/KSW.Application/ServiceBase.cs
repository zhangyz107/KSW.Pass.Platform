using KSW.Data.Abstractions;

namespace KSW.Application;

/// <summary>
/// 应用服务
/// </summary>
public abstract class ServiceBase : IService
{
    /// <summary>
    /// 初始化应用服务
    /// </summary>
    /// <param name="containerProvider">服务提供器</param>
    protected ServiceBase(IContainerProvider containerProvider, IUnitOfWork unitOfWork)
    {
        ContainerProvider = containerProvider ?? throw new ArgumentNullException(nameof(containerProvider));
        UnitOfWork = unitOfWork;
    }

    /// <summary>
    /// 容器
    /// </summary>
    protected IContainerProvider ContainerProvider { get; }

    /// <summary>
    /// 工作单元
    /// </summary>
    protected IUnitOfWork UnitOfWork { get; }
}