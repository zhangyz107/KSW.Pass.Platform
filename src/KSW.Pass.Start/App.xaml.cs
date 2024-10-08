using KSW.Infrastructure;
using KSW.Pass.Sqlite;
using KSW.Pass.Start.Views;
using Serilog;
using System.Windows;
using System.Windows.Threading;

namespace KSW.Pass.Platform
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }
        protected override void OnInitialized()
        {
            Log.Information("应用程序开始运行");

            base.OnInitialized();
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<ShellView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.GetContainer();
            InitLogConfig();
            // 将 Serilog 注入容器
            containerRegistry.RegisterInstance(Log.Logger);

            containerRegistry.Register<Dispatcher>(() => Current.Dispatcher);

            var bootstrapper = new Bootstrapper(containerRegistry);
            bootstrapper.Start();
        }

        private void InitLogConfig()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            //添加Sqlite模块
            moduleCatalog.AddModule<SqliteModule>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("应用程序退出运行");

            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }

}
