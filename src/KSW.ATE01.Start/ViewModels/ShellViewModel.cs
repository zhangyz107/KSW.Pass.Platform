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
using KSW.ATE01.Application.Events.Projects;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Start.Views;
using KSW.ATE01.Start.Views.Dialogs;
using KSW.Ui;
using MaterialDesignColors;
using MaterialDesignColors.ColorManipulation;
using MaterialDesignColors.Recommended;
using MaterialDesignThemes.Wpf;
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
        private readonly IProjectBLL _projectBLL;
        private readonly PaletteHelper _paletteHelper = new();
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

        private DelegateCommand _newProjectCommand;
        public DelegateCommand NewProjectCommand =>
            _newProjectCommand ?? (_newProjectCommand = new DelegateCommand(ExecuteNewProjectCommand));

        private DelegateCommand _saveAsCommand;
        public DelegateCommand SaveAsCommand =>
            _saveAsCommand ?? (_saveAsCommand = new DelegateCommand(ExecuteSaveAsCommand, () => _projectBLL?.GetCurrentProjectInfo() != null));

        private DelegateCommand _releaseCommand;
        public DelegateCommand ReleaseCommand =>
            _releaseCommand ?? (_releaseCommand = new DelegateCommand(ExecuteReleaseCommand, () => _projectBLL?.GetCurrentProjectInfo() != null));
        #endregion

        public ShellViewModel(
            IContainerExtension containerProvider,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IProjectBLL projectBLL
            ) : base(containerProvider)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _projectBLL = projectBLL;

            _regionManager.RegisterViewWithRegion(RegionNameManagement.ProjectViewContent, typeof(ProjectView));

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
            _eventAggregator.GetEvent<ProjectInfoUpdateEvent>().Subscribe(ProjectInfoUpdate);
        }

        private void ProjectInfoUpdate()
        {
            SaveAsCommand.RaiseCanExecuteChanged();
            ReleaseCommand.RaiseCanExecuteChanged();
        }

        private void ExecuteLoadingCommand()
        {
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

        private void ExecuteNewProjectCommand()
        {
            DialogService.ShowDialog(nameof(NewProjectDialog));
        }

        private void ExecuteSaveAsCommand()
        {
            DialogService.ShowDialog(nameof(SaveAsDialog));
        }

        private void ExecuteReleaseCommand()
        {
            DialogService.ShowDialog(nameof(ReleaseDialog));
        }
    }
}
