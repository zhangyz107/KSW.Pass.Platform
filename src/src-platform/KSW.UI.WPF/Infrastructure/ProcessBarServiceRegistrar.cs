using KSW.Infrastructure;
using KSW.ObjectMapping;
using KSW.UI.WPF.ViewModels;
using KSW.UI.WPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.UI.WPF.Infrastructure
{
    /// <summary>
    /// 进度条服务注册器
    /// </summary>
    public class ProcessBarServiceRegistrar : IServiceRegistrar
    {
        /// <summary>
        /// 获取服务名
        /// </summary>
        public static string ServiceName => "KSW.UI.WPF.Infrastructure.ProcessBarServiceRegistrar";

        public int OrderId => 400;

        public bool Enabled => ServiceRegistrarConfig.IsEnabled(ServiceName);

        public Action Register(ServiceContext context)
        {
            context.ContainerRegistry.RegisterDialog<ProcessBarDialog, ProcessBarDialogViewModel>();
            context.ContainerRegistry.RegisterDialog<MessageDialog, MessageDialogViewModel>();
            return null;
        }
    }
}
