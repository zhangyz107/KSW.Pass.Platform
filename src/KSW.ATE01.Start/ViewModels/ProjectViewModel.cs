/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：ProjectViewModel.cs
// 功能描述：项目视图模型
//
// 作者：zhangyingzhong
// 日期：2024/10/09 13:46
// 修改记录(Revision History)
// 修改时间：2025/08/12 16:45
// 修改人：zhangyingzhong
//------------------------------------------------------------*/

using KSW.ATE01.Application.BLLs.Abstractions.Projects;
using KSW.ATE01.Application.Events.Projects;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Start.Views;
using KSW.ATE01.Start.Views.Dialogs;
using KSW.ATE01.Start.Views.TestPlans;
using KSW.Helpers;
using KSW.Ui;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace KSW.ATE01.Start.ViewModels
{
    /// <summary>
    /// 项目视图模型
    /// </summary>
    public class ProjectViewModel : ViewModelBase
    {
        #region Fields
        private readonly IContainerExtension _containerProvider;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly IProjectBLL _projectBLL;
        private ObservableCollection<ProjectInfoModel> _projectList;
        private ProjectDetailView _projectDetailView;
        private ProjectInfoModel _selectProjectInfo;
        #endregion

        #region Properties
        public ObservableCollection<ProjectInfoModel> ProjectList
        {
            get => _projectList;
            set => SetProperty(ref _projectList, value);
        }

        public ProjectInfoModel SelectProjectInfo
        {
            get => _selectProjectInfo;
            set
            {
                if (SetProperty(ref _selectProjectInfo, value))
                {
                    _projectBLL?.SetCurrentProjectInfo(value);
                    _eventAggregator.GetEvent<SelectedProjectInfoEvent>().Publish();
                }
            }
        }

        public ProjectDetailView ProjectDetailView
        {
            get => _projectDetailView;
            set => SetProperty(ref _projectDetailView, value);
        }

        #endregion

        #region Command
        private AsyncDelegateCommand _loadingCommand;
        public AsyncDelegateCommand LoadingCommand =>
            _loadingCommand ?? (_loadingCommand = new AsyncDelegateCommand(ExecuteLoadingCommand));

        private DelegateCommand _newProjectCommand;
        public DelegateCommand NewProjectCommand =>
            _newProjectCommand ?? (_newProjectCommand = new DelegateCommand(ExecuteNewProjectCommand));

        private DelegateCommand _openProjectCommand;
        public DelegateCommand OpenProjectCommand =>
            _openProjectCommand ?? (_openProjectCommand = new DelegateCommand(ExecuteOpenProjectCommand));

        private DelegateCommand _saveAsCommand;
        public DelegateCommand SaveAsCommand =>
            _saveAsCommand ?? (_saveAsCommand = new DelegateCommand(ExecuteSaveAsCommand));

        private DelegateCommand _delelopCommand;
        public DelegateCommand DelelopCommand =>
            _delelopCommand ?? (_delelopCommand = new DelegateCommand(ExecuteDelelopCommand));

        private DelegateCommand _runCommand;
        public DelegateCommand RunCommand =>
            _runCommand ?? (_runCommand = new DelegateCommand(ExecuteRunCommand));

        private DelegateCommand _releaseCommand;
        public DelegateCommand ReleaseCommand =>
            _releaseCommand ?? (_releaseCommand = new DelegateCommand(ExecuteReleaseCommand));

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public ProjectViewModel(
            IContainerExtension containerProvider,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            IProjectBLL projectBLL) : base(containerProvider)
        {
            _containerProvider = containerProvider;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _projectBLL = projectBLL;

            #region 加载页面
            _projectDetailView = _containerProvider.Resolve<ProjectDetailView>();
            #endregion
        }

        private async Task ExecuteLoadingCommand()
        {
            var list = await _projectBLL?.GetListAsync();
            ProjectList = new ObservableCollection<ProjectInfoModel>(list);
            foreach (var item in _projectList)
            {
                item.DelelopCommand = DelelopCommand;
                item.RunCommand = RunCommand;
            }

            _regionManager.RequestNavigate(RegionNameManagement.TestPlanContent, nameof(ChannelSettingView));
            _regionManager.RequestNavigate(RegionNameManagement.TestPlanContent, nameof(LimitsSettingView));
            _regionManager.RequestNavigate(RegionNameManagement.TestPlanContent, nameof(LevelSettingView));
            _regionManager.RequestNavigate(RegionNameManagement.TestPlanContent, nameof(TimingSettingView));
        }

        private void ExecuteNewProjectCommand()
        {
            DialogService.ShowDialog(nameof(NewProjectDialog));
        }

        private void ExecuteOpenProjectCommand()
        {
            DialogService.ShowDialog(nameof(OpenProjectDialog));
        }
        private void ExecuteSaveAsCommand()
        {
            DialogService.ShowDialog(nameof(SaveAsDialog));
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

        private void ExecuteReleaseCommand()
        {
            DialogService.ShowDialog(nameof(ReleaseDialog));
        }
    }
}
