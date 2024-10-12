using KSW.Data.Abstractions;
using KSW.Localization;
using Microsoft.Extensions.Logging;

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
    protected ServiceBase(IContainerProvider containerProvider)
    {
        ContainerProvider = containerProvider ?? throw new ArgumentNullException(nameof(containerProvider));
        L = containerProvider.Resolve<ILanguageManager>() ?? throw new ArgumentNullException(nameof(ILanguageManager));
        var logFactory = containerProvider.Resolve<ILoggerFactory>() ?? throw new ArgumentNullException(nameof(ILoggerFactory)); ;
        Logger = logFactory?.CreateLogger(GetType());
    }

    /// <summary>
    /// 容器
    /// </summary>
    protected IContainerProvider ContainerProvider { get; }

    /// <summary>
    /// 多语言
    /// </summary>
    protected ILanguageManager L { get; }

    /// <summary>
    /// 日志记录
    /// </summary>
    protected ILogger Logger { get; }
}