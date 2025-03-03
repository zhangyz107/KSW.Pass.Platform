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
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;

namespace KSW.ATE01.Platform
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        private static Mutex mutex;

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const string _title = "ATE01";
        private const int SW_RESTORE = 9;
        private const int SW_SHOW = 5;

        protected override void OnStartup(StartupEventArgs e)
        {
            bool createNew;
            mutex = new Mutex(true, "Singleton", out createNew);
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            if (!createNew)
            {
                MessageBox.Show("软件已启动，不再重复启动。", _title, MessageBoxButton.OK, MessageBoxImage.Asterisk);

                //// 找到已运行的实例并激活
                //Process currentProcess = Process.GetCurrentProcess();
                //Process existingProcess = Process.GetProcessesByName(currentProcess.ProcessName)
                //                               .FirstOrDefault(p => p.Id != currentProcess.Id);

                //if (existingProcess != null)
                //{
                //    IntPtr hWnd = existingProcess.MainWindowHandle;
                //    if (hWnd != IntPtr.Zero)
                //    {
                //        ShowWindow(hWnd, SW_RESTORE);
                //        SetForegroundWindow(hWnd);
                //    }
                //    else
                //    {
                //        // 这里假设你知道窗口的类名或标题
                //        hWnd = FindWindow(null, _title);  // 根据窗口标题查找
                //        if (hWnd != IntPtr.Zero)
                //        {
                //            ShowWindow(hWnd, SW_SHOW);
                //            ShowWindow(hWnd, SW_RESTORE);
                //            SetForegroundWindow(hWnd);
                //        }
                //    }
                //}
                Environment.Exit(1);
            }
            else
            {
                base.OnStartup(e);
                if (e.Args.Any())
                {
                    var mainWindow = Current.MainWindow as ShellView;
                    mainWindow.ExecuteMethodBasedOnArgument(e.Args.FirstOrDefault());
                }

            }

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
            containerRegistry.RegisterDialog<ConfigureDialog, ConfigureDialogViewModel>();
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
