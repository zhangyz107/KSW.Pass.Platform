using DryIoc.Microsoft.DependencyInjection;
using KSW.ATE01.Sqlite;
using KSW.ATE01.Start;
using KSW.ATE01.Start.ViewModels.Dialogs;
using KSW.ATE01.Start.Views;
using KSW.ATE01.Start.Views.Dialogs;
using KSW.Infrastructure;
using KSW.Localization;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Configuration;
using System.Windows;
using System.Windows.Threading;

namespace KSW.ATE01.Platform
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            base.OnStartup(e);
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error(e.Exception, e.Exception.Message);
            MessageBox.Show(e.Exception.Message);

            e.Handled = true;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<ShellView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var container = containerRegistry.GetContainer();

            // 初始化日志配置
            InitLogConfig();
            
            // 初始化多语言配置
            InitLanguageConfig(containerRegistry);

            // 将 Serilog 注入容器
            containerRegistry.RegisterInstance(Log.Logger);

            containerRegistry.Register<Dispatcher>(() => Current.Dispatcher);

            var bootstrapper = new Bootstrapper(containerRegistry);
            bootstrapper.Start();

            RegisterView(containerRegistry);
        }


        protected override IContainerExtension CreateContainerExtension()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));

            return new DryIocContainerExtension(new Container(CreateContainerRules())
    .WithDependencyInjectionAdapter(serviceCollection));
        }

        private void RegisterView(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ProjectView>();
            containerRegistry.Register<HelpView>();
            containerRegistry.Register<ProjectDetailView>();

            containerRegistry.RegisterDialog<NewProjectDialog, NewProjectDialogViewModel>();
            containerRegistry.RegisterDialog<OpenProjectDialog, OpenProjectDialogViewModel>();
            containerRegistry.RegisterDialog<SaveAsDialog, SaveAsDialogViewModel>();
            containerRegistry.RegisterDialog<ReleaseDialog, ReleaseDialogViewModel>();
            containerRegistry.RegisterDialog<RunDialog, RunDialogViewModel>();
        }

        private void InitLogConfig()
        {
            var logOutputTemplate = ConfigurationManager.AppSettings["OutputTemplate"];

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, outputTemplate: logOutputTemplate)
                .CreateLogger();
        }

        private void InitLanguageConfig(IContainerRegistry containerRegistry)
        {
            var languageManager = LanguageManager.Instance;
            containerRegistry.RegisterInstance<ILanguageManager>(languageManager);
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            //添加Sqlite模块
            moduleCatalog.AddModule<SqliteModule>();
        }


        protected override void OnExit(ExitEventArgs e)
        {
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }

}
