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
using KSW.ATE01.Application.Events.Projects;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Start.Views;
using KSW.ATE01.Start.Views.Dialogs;
using KSW.Helpers;
using KSW.Ui;
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
    public class ShellViewModel : ViewModelBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly PaletteHelper _paletteHelper = new();
        private IProjectBLL _projectBLL;
        private bool _isChinese;

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
        }

        private void SelectedProjectInfo()
        {
            SaveAsCommand.RaiseCanExecuteChanged();
            ReleaseCommand.RaiseCanExecuteChanged();
            DelelopCommand.RaiseCanExecuteChanged();
            RunCommand.RaiseCanExecuteChanged();

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
            LanguageManager.Instance.ChangeLanguage(culture);
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
                _eventAggregator.GetEvent<ProjectInfoUpdateEvent>().Publish();
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
            var currentProjectInfo = _projectBLL?.GetCurrentProjectInfo();
            if (currentProjectInfo == null)
            {
                await DialogService.ShowMessageDialog("未打开项目!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }
            DialogService.ShowDialog(nameof(RunDialog));
        }
    }
}
