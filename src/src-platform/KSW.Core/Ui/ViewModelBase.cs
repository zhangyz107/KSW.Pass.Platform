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
            DialogService = containerProvider.IsRegistered<IDialogService>() ? containerProvider?.Resolve<IDialogService>() : null;
            L = containerProvider.IsRegistered<ILanguageManager>() == true ? containerProvider.Resolve<ILanguageManager>() : null;
            var logFactory = containerProvider.Resolve<ILoggerFactory>() ?? throw new ArgumentNullException(nameof(ILoggerFactory));
            Log = logFactory?.CreateLogger(GetType());
        }

        /// <summary>
        /// 容器
        /// </summary>
        protected IContainerProvider ContainerProvider { get; }

        /// <summary>
        /// 对话框服务
        /// </summary>
        protected IDialogService DialogService { get; }

        /// <summary>
        /// 多语言
        /// </summary>
        protected ILanguageManager L { get; }

        /// <summary>
        /// 日志记录
        /// </summary>
        protected ILogger Log { get; }

        /// <summary>
        /// 执行异常处理
        /// </summary>
        protected async Task ExecuteWithExceptionHandling(Action action, Func<Exception, Task> errorCallBack = null)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                await HandleException(ex, errorCallBack);
            }
        }

        /// <summary>
        /// 执行异常处理
        /// </summary>
        protected async Task ExecuteWithExceptionHandling(Func<Task> action, Func<Exception, Task> errorCallBack = null)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                await HandleException(ex, errorCallBack);
            }
        }

        /// <summary>
        /// 处理异常
        /// </summary>
        protected virtual async Task HandleException(Exception ex, Func<Exception, Task> errorCallBack = null)
        {
            Log?.LogError(ex, ex.Message);
            await errorCallBack(ex);
        }
    }
}
