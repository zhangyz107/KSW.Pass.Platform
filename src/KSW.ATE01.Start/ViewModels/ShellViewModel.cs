/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：ShellViewModel.cs
// 功能描述：主窗口视图模型
//
// 作者：zhangyingzhong
// 日期：2024/10/09 13:41
// 修改记录(Revision History)
// 修改时间：2025/08/12 16:45
// 修改人：zhangyingzhong
//------------------------------------------------------------*/


using KSW.ATE01.Application.BLLs.Abstractions.Projects;
using KSW.ATE01.Application.BLLs.Implements.Projects;
using KSW.ATE01.Application.Events;
using KSW.ATE01.Application.Events.Projects;
using KSW.ATE01.Domain.Projects.Core.Enums;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Start.Views;
using KSW.ATE01.Start.Views.Dialogs;
using KSW.ATE01.Start.Views.Patterns;
using KSW.ATE01.Start.Views.TestPlans;
using KSW.Helpers;
using KSW.Localization;
using KSW.Ui;
using KSW.UI.WPF.Controls;
using MaterialDesignColors;
using MaterialDesignColors.ColorManipulation;
using MaterialDesignColors.Recommended;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Windows.Media;

namespace KSW.ATE01.Start.ViewModels
{
    /// <summary>
    /// 主窗口视图模型
    /// </summary>
    public class ShellViewModel : ViewModelBase, IToastViewModel
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly PaletteHelper _paletteHelper = new();
        private IProjectBLL _projectBLL;
        private bool _isChinese;
        private bool _showProjectView = true;
        private bool _showBackwardView = false;
        private bool _showRunView = false;
        private bool _showPatterToolView = false;
        private string _selectProject;

        #region Properties
        public bool IsChinese
        {
            get => _isChinese;
            set
            {
                if (SetProperty(ref _isChinese, value))
                {
                    ChangeLanguage(value ? "zh-CN" : "en-US");
                }
            }
        }

        public string Title { get => "ATE01"; }

        /// <summary>
        /// 显示运行视图
        /// </summary>
        public bool ShowRunView
        {
            get => _showRunView;
            set => SetProperty(ref _showRunView, value);
        }

        /// <summary>
        /// 显示返回视图
        /// </summary>
        public bool ShowBackwardView
        {
            get => _showBackwardView;
            set => SetProperty(ref _showBackwardView, value);
        }


        /// <summary>
        /// 显示项目视图
        /// </summary>
        public bool ShowProjectView
        {
            get => _showProjectView;
            set => SetProperty(ref _showProjectView, value);
        }

        /// <summary>
        /// 显示向量工具视图
        /// </summary>
        public bool ShowPatterToolView
        {
            get => _showPatterToolView;
            set => SetProperty(ref _showPatterToolView, value);
        }


        /// <summary>
        /// 选中项目
        /// </summary>
        public string SelectProject
        {
            get => _selectProject;
            set => SetProperty(ref _selectProject, value);
        }

        /// <summary>
        /// Toast管理器
        /// </summary>
        public WindowToastManager? ToastManager { get; set; }

        #endregion

        #region Command
        private DelegateCommand _loadingCommand;
        public DelegateCommand LoadingCommand =>
            _loadingCommand ?? (_loadingCommand = new DelegateCommand(ExecuteLoadingCommand));

        private AsyncDelegateCommand _newProjectCommand;
        public AsyncDelegateCommand NewProjectCommand =>
            _newProjectCommand ?? (_newProjectCommand = new AsyncDelegateCommand(ExecuteNewProjectCommand));

        private AsyncDelegateCommand _saveAsCommand;
        public AsyncDelegateCommand SaveAsCommand =>
            _saveAsCommand ?? (_saveAsCommand = new AsyncDelegateCommand(ExecuteSaveAsCommand, () => _projectBLL?.GetCurrentProjectInfo() != null));

        private AsyncDelegateCommand _releaseCommand;
        public AsyncDelegateCommand ReleaseCommand =>
            _releaseCommand ?? (_releaseCommand = new AsyncDelegateCommand(ExecuteReleaseCommand, () => _projectBLL?.GetCurrentProjectInfo() != null));

        private DelegateCommand _delelopCommand;
        public DelegateCommand DelelopCommand =>
            _delelopCommand ?? (_delelopCommand = new DelegateCommand(ExecuteDelelopCommand, () => _projectBLL?.GetCurrentProjectInfo() != null));

        private DelegateCommand _runCommand;
        public DelegateCommand RunCommand =>
            _runCommand ?? (_runCommand = new DelegateCommand(ExecuteRunCommand, () => _projectBLL?.GetCurrentProjectInfo() != null));

        private DelegateCommand _backwardCommand;
        public DelegateCommand BackwardCommand =>
            _backwardCommand ?? (_backwardCommand = new DelegateCommand(ExecuteBackwardCommand));

        private DelegateCommand _patternToolCommand;
        public DelegateCommand PatternToolCommand =>
            _patternToolCommand ?? (_patternToolCommand = new DelegateCommand(ExecutePatternToolCommand));

        #endregion

        public ShellViewModel(
            IContainerExtension containerProvider,
            IRegionManager regionManager,
            IEventAggregator eventAggregator
            ) : base(containerProvider)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            _regionManager.RegisterViewWithRegion(RegionNameManagement.ProjectViewContent, typeof(ProjectView));
            _regionManager.RegisterViewWithRegion(RegionNameManagement.ProjectDetailContent, typeof(ProjectDetailView));
            _regionManager.RegisterViewWithRegion(RegionNameManagement.RunViewContent, typeof(RunDialog));
            _regionManager.RegisterViewWithRegion(RegionNameManagement.PatternToolContent, typeof(PatternToolView));

            Theme theme = _paletteHelper.GetTheme();

            if (!ATE01ShareMemory.OpenShareMemory())
            {
                //开启共享内存
                ATE01ShareMemory.CreateShareMemory();
            }

            RegisterEvent();
        }

        private void RegisterEvent()
        {
            _eventAggregator.GetEvent<SelectedProjectInfoEvent>().Subscribe(SelectedProjectInfo);
            _eventAggregator.GetEvent<ChangeMainViewEvent>().Subscribe(ChangeMainView, ThreadOption.UIThread);
            _eventAggregator.GetEvent<ShowShellToastEvent>().Subscribe(ShowToast, ThreadOption.UIThread);
        }

        private void SelectedProjectInfo()
        {
            SaveAsCommand.RaiseCanExecuteChanged();
            ReleaseCommand.RaiseCanExecuteChanged();
            DelelopCommand.RaiseCanExecuteChanged();
            RunCommand.RaiseCanExecuteChanged();
            SelectProject = $"{L["CurrentProject"]}：{_projectBLL?.GetCurrentProjectInfo()?.ProjectName}";
        }

        private void ChangeMainView(MainViewType type)
        {
            switch (type)
            {
                case MainViewType.ProjectView:
                    ExecuteBackwardCommand();
                    break;
                case MainViewType.RunView:
                    ExecuteRunCommand();
                    break;
            }
        }

        private void ShowToast(object obj)
        {
            if (obj is Toast model)
            {
                ToastManager?.Show(model);
            }
        }

        private void ExecuteLoadingCommand()
        {
            _projectBLL = ContainerProvider?.Resolve<ProjectBLL>();

            ChangePrimaryColor(BlueSwatch.Blue300);

            var currentCulture = CultureInfo.CurrentCulture;
            switch (currentCulture.Name)
            {
                case "zh-CN":
                    IsChinese = true;
                    break;
                case "en-US":
                    IsChinese = false;
                    break;
                default:
                    break;
            }
        }


        private void ChangeLanguage(string value)
        {
            CultureInfo culture = new CultureInfo(value);
            LanguageManagerFactory.ChangeLanguage(culture);

            // 切换核心多语言
            //Language.LanguageManager.Instance.ChangeLanguage(culture);
        }

        private void ChangePrimaryColor(Color color)
        {
            Theme theme = _paletteHelper.GetTheme();

            theme.PrimaryLight = new ColorPair(color.Lighten());
            theme.PrimaryMid = new ColorPair(color);
            theme.PrimaryDark = new ColorPair(color.Darken());
            theme.SetPrimaryColor(color);
            _paletteHelper.SetTheme(theme);
        }

        private async Task ExecuteNewProjectCommand()
        {
            if ((await DialogService.ShowDialogAsync(nameof(NewProjectDialog)))?.Result == ButtonResult.OK)
            {
                _eventAggregator.GetEvent<RefreshProjectListEvent>().Publish();
            }
        }

        private async Task ExecuteSaveAsCommand()
        {
            if ((await DialogService.ShowDialogAsync(nameof(SaveAsDialog)))?.Result == ButtonResult.OK)
            {
                _eventAggregator.GetEvent<RefreshProjectListEvent>().Publish();
            }
        }

        private async Task ExecuteReleaseCommand()
        {
            if ((await DialogService.ShowDialogAsync(nameof(ReleaseDialog)))?.Result == ButtonResult.OK)
                _eventAggregator.GetEvent<UpdateProjectInfoEvent>().Publish();
        }

        private void ExecuteDelelopCommand()
        {
            try
            {
                _projectBLL.RunProjecctByVS();
            }
            catch (Exception e)
            {
                DialogService.ShowMessageDialog(e.Message);
                Log?.LogError(e, e.Message);
            }
        }

        private async void ExecuteRunCommand()
        {
            ShowProjectView = false;
            ShowBackwardView = true;
            ShowRunView = true;
            ShowPatterToolView = false;
        }

        private void ExecuteBackwardCommand()
        {
            ShowProjectView = true;
            ShowBackwardView = false;
            ShowRunView = false;
            ShowPatterToolView = false;
        }

        private void ExecutePatternToolCommand()
        {
            ShowProjectView = false;
            ShowBackwardView = true;
            ShowRunView = false;
            ShowPatterToolView = true;
        }

    }
}
