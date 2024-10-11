using KSW.Dtos;
using KSW.Localization;

namespace KSW.Ui
{
    /// <summary>
    /// 视图模型基类
    /// </summary>
    public abstract class ViewModelBase : DtoBase
    {
        protected IContainerProvider ContainerProvider { get; }

        protected ILanguageManager L { get; }

        protected ViewModelBase(IContainerProvider containerProvider)
        {
            ContainerProvider = containerProvider ?? throw new ArgumentNullException(nameof(containerProvider)); ;
            L = containerProvider.Resolve<ILanguageManager>();
        }
    }
}
