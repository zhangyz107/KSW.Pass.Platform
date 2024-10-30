using EnvDTE;
using EnvDTE80;
using KSW.ATE01.Extension.Shared.Helpers;
using KSW.ATE01.Extension.Shared.Integration.Commands;
using KSW.ATE01.Extension.Shared.Integration.Events;
using KSW.ATE01.Extension.Shared.Properties;
using KSW.ATE01.Extension.VS2022;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KSW.ATE01.Extension.Shared
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)] //告诉 Visual Studio 实用程序这是一个需要注册的包。
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExistsAndFullyLoaded_string, PackageAutoLoadFlags.BackgroundLoad)] //触发 Pass 在解决方案打开时加载，以便菜单项可以确定其状态。
    [ProvideBindingPath]    //加载程序集
    [ProvideMenuResource("Menus.ctmenu", 1)] //需要此属性来让 shell 知道此包公开了一些菜单。
    [Guid(ATE01Guids.GuidATE01PackageString)]
    public class ATE01Package : AsyncPackage
    {
        /// <summary>
        /// The IComponentModel service.
        /// </summary>
        private IComponentModel _componentModel;

        /// <summary>
        /// The top level application instance of the VS IDE that is executing this package.
        /// </summary>
        private DTE2 _ide;

        /// <summary>
        /// Gets the top level application instance of the VS IDE that is executing this package.
        /// </summary>
        public DTE2 IDE => _ide ?? (_ide = (DTE2)GetService(typeof(DTE)));
        /// <summary>
        /// Gets or sets the settings monitor.
        /// </summary>
        public SettingsMonitor<Settings> SettingsMonitor { get; private set; }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            SettingsMonitor = new SettingsMonitor<Settings>(Settings.Default, JoinableTaskFactory);

            await RegisterCommandsAsync();
            var settingsContextHelper = SettingsContextHelper.GetInstance(this);
            await SolutionEventListener.InitializeAsync(this);

            SolutionEventListener.Instance.OnSolutionOpened += settingsContextHelper.OnSolutionOpened;
            SolutionEventListener.Instance.OnSolutionClosed += settingsContextHelper.OnSolutionClosed;
        }

        private async Task RegisterCommandsAsync()
        {
            await TestPlanCommand.InitializeAsync(this);
            await PpmuDteCommand.InitializeAsync(this);
            await DigitalDteCommand.InitializeAsync(this);
            await DpsDteCommand.InitializeAsync(this);
            await UdbDteCommand.InitializeAsync(this);
        }
    }
}
