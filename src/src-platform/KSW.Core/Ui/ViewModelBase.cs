using KSW.Dtos;
using KSW.Localization;
using Microsoft.Extensions.Logging;

namespace KSW.Ui
{
    /// <summary>
    /// 视图模型基类
    /// </summary>
    public abstract class ViewModelBase : DtoBase
    {
        protected ViewModelBase(IContainerProvider containerProvider)
        {
            ContainerProvider = containerProvider ?? throw new ArgumentNullException(nameof(containerProvider));
            L = containerProvider.Resolve<ILanguageManager>() ?? throw new ArgumentNullException(nameof(ILanguageManager));
            var logFactory = containerProvider.Resolve<ILoggerFactory>() ?? throw new ArgumentNullException(nameof(ILoggerFactory));
            Log = logFactory?.CreateLogger(GetType());
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
        protected ILogger Log { get; }
    }
}
