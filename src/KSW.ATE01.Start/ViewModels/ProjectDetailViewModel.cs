/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：ProjectDetailViewModel.cs
// 功能描述：项目明细视图模型
//
// 作者：zhangyingzhong
// 日期：2024/10/09 13:41
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.ATE01.Application.BLLs.Abstractions.Projects;
using KSW.ATE01.Application.Events.Projects;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Start.Views.Dialogs;
using KSW.Ui;
using Microsoft.Win32;
using System.IO;

namespace KSW.ATE01.Start.ViewModels
{
    /// <summary>
    /// 项目明细视图模型
    /// </summary>
    public class ProjectDetailViewModel : ViewModelBase
    {
        #region Fields
        private readonly IContainerExtension _containerProvider;
        private readonly IEventAggregator _eventAggregator;
        private IProjectBLL _projectBLL;
        private ProjectInfoModel _projectInfo;
        private string _testPlanName;
        private string _executeName;
        private bool _projectPathVaild;
        #endregion

        #region Properties
        public ProjectInfoModel ProjectInfo
        {
            get => _projectInfo;
            private set => SetProperty(ref _projectInfo, value);
        }

        public string TestPlanName
        {
            get => _testPlanName;
            set => SetProperty(ref _testPlanName, value);
        }

        public string ExecuteName
        {
            get => _executeName;
            set => SetProperty(ref _executeName, value);
        }

        /// <summary>
        /// 项目路径是否有效
        /// </summary>
        public bool ProjectPathVaild =>  (_projectInfo?.ProjectPath?.IsEmpty() == false);

        #endregion

        #region Commands
        private AsyncDelegateCommand _loadingCommand;
        public AsyncDelegateCommand LoadingCommand =>
            _loadingCommand ?? (_loadingCommand = new AsyncDelegateCommand(ExecuteLoadingCommand));

        private DelegateCommand _openFolderCommand;
        public DelegateCommand OpenFolderCommand =>
            _openFolderCommand ?? (_openFolderCommand = new DelegateCommand(ExecuteOpenFolderCommand));
        #endregion

        public ProjectDetailViewModel(
            IContainerExtension containerProvider,
            IEventAggregator eventAggregator) : base(containerProvider)
        {
            _containerProvider = containerProvider;
            _eventAggregator = eventAggregator;

            RegisterEvent();
        }

        private async Task ExecuteLoadingCommand()
        {
            _projectBLL = _containerProvider?.Resolve<IProjectBLL>();
        }

        private void RegisterEvent()
        {
            _eventAggregator.GetEvent<SelectedProjectInfoEvent>().Subscribe(SelectedProjectInfo);
            _eventAggregator.GetEvent<UpdateProjectInfoEvent>().Subscribe(ProjectInfoUpdate);
            _eventAggregator.GetEvent<LoadProjectFromArgsEvent>().Subscribe(LoadProjectFromArgs);
        }

        private void SelectedProjectInfo()
        {
            ProjectInfoUpdate();
        }

        private void ProjectInfoUpdate()
        {
            ProjectInfo = _projectBLL?.GetCurrentProjectInfo();
            ExecuteName = _projectInfo?.ProjectName + _projectInfo?.ExecuteExtension;
            RaisePropertyChanged(nameof(ProjectPathVaild));
        }

        private void LoadProjectFromArgs(string dir)
        {
            var folder = new DirectoryInfo(dir);
            if (folder.Exists)
            {
                var cfgs = folder.GetFiles("*.atecfg");
                if (cfgs.Any())
                {
                    var cfgFile = cfgs.FirstOrDefault();
                    if (cfgFile != null)
                    {
                        //ProjectInfo = _projectBLL.LoadProjectInfo(cfgFile.FullName);
                        _projectBLL.SetCurrentProjectInfo(ProjectInfo);
                        //TestPlanName = _projectInfo.ProjectName + _projectInfo.TestPlanExtension;
                        ExecuteName = _projectInfo.ProjectName + _projectInfo.ExecuteExtension;
                        DialogService.ShowDialog(nameof(RunDialog));
                    }
                }
            }
        }

        private void ExecuteOpenFolderCommand()
        {
            _projectBLL?.OpenFolder();
        }
    }
}
