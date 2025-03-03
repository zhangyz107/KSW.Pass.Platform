using DryIoc.Microsoft.DependencyInjection;
using KSW.ATE01.PPMU.Start.Language;
using KSW.ATE01.PPMU.Start.Views;
using KSW.Infrastructure;
using KSW.Localization;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;

namespace KSW.ATE01.PPMU.DTE
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        private static Mutex mutex;

        private const string _title = "PPMU DTE";

        protected override void OnStartup(StartupEventArgs e)
        {
            bool createNew;
            mutex = new Mutex(true, "Singleton", out createNew);
            if (!createNew)
            {
                MessageBox.Show("软件已启动，不再重复启动。", _title, MessageBoxButton.OK, MessageBoxImage.Asterisk);
                Environment.Exit(1);
            }
            else
            {
                base.OnStartup(e);
            }
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


        private void InitLogConfig()
        {
            var logOutputTemplate = ConfigurationManager.AppSettings["OutputTemplate"];
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.File($"logs/log-{applicationName}-.txt", rollingInterval: RollingInterval.Day, outputTemplate: logOutputTemplate)
                .CreateLogger();
        }

        private void InitLanguageConfig(IContainerRegistry containerRegistry)
        {
            var languageManager = LanguageManager.Instance;
            containerRegistry.RegisterInstance<ILanguageManager>(languageManager);
        }


        private void RegisterView(IContainerRegistry containerRegistry)
        {

        }

        protected override IContainerExtension CreateContainerExtension()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));

            return new DryIocContainerExtension(new Container(CreateContainerRules())
    .WithDependencyInjectionAdapter(serviceCollection));
        }
    }

}
