using DryIoc.Microsoft.DependencyInjection;
using KSW.ATE01.Sqlite;
using KSW.ATE01.Start;
using KSW.ATE01.Start.ViewModels.Dialogs;
using KSW.ATE01.Start.ViewModels.Dialogs.TestPlans;
using KSW.ATE01.Start.Views;
using KSW.ATE01.Start.Views.Dialogs;
using KSW.ATE01.Start.Views.Dialogs.TestPlans;
using KSW.ATE01.Start.Views.TestPlans;
using KSW.Helpers;
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

                ApplySystemTheme();

                if (e.Args.Any())
                {
                    var mainWindow = Current.MainWindow as ShellView;
                    mainWindow.ExecuteMethodBasedOnArgument(e.Args.FirstOrDefault());
                }

            }

        }

        private void ApplySystemTheme()
        {
            // 此处需要实现检测系统当前是浅色还是深色模式的逻辑
            // 以下是一个示例性的判断逻辑，实际应用中可能需要更完善的检查
            bool isDark = IsSystemDarkThemeEnabled(); // 你需要实现这个检测方法
            UI.WPF.Themes.ThemeAssist.ChangeTheme(isDark); // 调用之前实现的切换主题方法
        }

        // 一个简单的示例方法，实际应用中你可能需要通过注册表或Windows API更精确地判断
        private bool IsSystemDarkThemeEnabled()
        {
            // 示例逻辑：检查 "AppsUseLightTheme" 注册表值，0通常表示深色，1表示浅色
            // 注意：此代码为示例，实际使用需添加异常处理等
            using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
            {
                var value = key?.GetValue("AppsUseLightTheme");
                if (value != null && value is int appsUseLightTheme)
                {
                    return appsUseLightTheme == 0; // 如果为0，则表示系统启用了深色主题
                }
            }
            return false; // 默认返回浅色
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

            var container = new DryIocContainerExtension(new Container(CreateContainerRules())
    .WithDependencyInjectionAdapter(serviceCollection));
            Ioc.SetServiceProviderAction(() => container);
            return container;
        }

        private void RegisterView(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ProjectView>();
            containerRegistry.Register<HelpView>();
            containerRegistry.Register<ProjectDetailView>();
            containerRegistry.Register<RunDialog>();

            containerRegistry.RegisterDialog<NewProjectDialog, NewProjectDialogViewModel>();
            containerRegistry.RegisterDialog<OpenProjectDialog, OpenProjectDialogViewModel>();
            containerRegistry.RegisterDialog<SaveAsDialog, SaveAsDialogViewModel>();
            containerRegistry.RegisterDialog<ReleaseDialog, ReleaseDialogViewModel>();
            //containerRegistry.RegisterDialog<RunDialog, RunDialogViewModel>();
            containerRegistry.RegisterDialog<ConfigureDialog, ConfigureDialogViewModel>();

            #region TestPlans
            containerRegistry.RegisterDialog<AddPinDialogView, AddPinDialogViewModel>();
            containerRegistry.RegisterDialog<GroupSettingDialogView, GroupSettingDialogViewModel>();
            containerRegistry.RegisterDialog<EditPinGroupDialogView, EditPinGroupDialogViewModel>();
            containerRegistry.RegisterDialog<AddLimitDialogView, AddLimitDialogViewModel>();
            containerRegistry.RegisterDialog<AddLevelDialogView, AddLevelDialogViewModel>();
            containerRegistry.RegisterDialog<AddTimingDialogView, AddTimingDialogViewModel>();
            containerRegistry.RegisterDialog<AddTestItemDialogView, AddTestItemDialogViewModel>();
            containerRegistry.RegisterDialog<AddGlobalParametersDialogView, AddGlobalParametersDialogViewModel>();

            containerRegistry.RegisterForNavigation<ChannelSettingView>();
            containerRegistry.RegisterForNavigation<LimitsSettingView>();
            containerRegistry.RegisterForNavigation<LevelSettingView>();
            containerRegistry.RegisterForNavigation<TimingSettingView>();
            containerRegistry.RegisterForNavigation<TestItemSettingView>();
            containerRegistry.RegisterForNavigation<GlobalSettingView>();
            #endregion
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
