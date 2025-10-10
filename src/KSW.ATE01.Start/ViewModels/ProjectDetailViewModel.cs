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
using KSW.ATE01.Domain.Projects.Core.Enums;
using KSW.Ui;
using System.Configuration;
using System.IO;

namespace KSW.ATE01.Start.ViewModels
{
    /// <summary>
    /// 项目明细视图模型
    /// </summary>
    public class ProjectDetailViewModel : ViewModelBase
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private IProjectBLL _projectBLL;
        private ProjectInfoModel _projectInfo;
        private string _executeName;
        #endregion

        #region Properties
        public ProjectInfoModel ProjectInfo
        {
            get => _projectInfo;
            private set => SetProperty(ref _projectInfo, value);
        }

        public string ExecuteName
        {
            get => _executeName;
            set => SetProperty(ref _executeName, value);
        }

        /// <summary>
        /// 项目路径是否有效
        /// </summary>
        public bool ProjectPathVaild => (_projectInfo?.ProjectPath?.IsEmpty() == false);

        #endregion

        #region Commands
        private DelegateCommand _openFolderCommand;
        public DelegateCommand OpenFolderCommand =>
            _openFolderCommand ?? (_openFolderCommand = new DelegateCommand(ExecuteOpenFolderCommand));
        #endregion

        public ProjectDetailViewModel(
            IContainerExtension containerProvider,
            IEventAggregator eventAggregator,
            IProjectBLL projectBLL) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
            _projectBLL = projectBLL;

            RegisterEvent();
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

        private async void LoadProjectFromArgs(string dir)
        {
            var projectConfigName = ConfigurationManager.AppSettings["ProjectConfigName"] ?? throw new ArgumentNullException("ProjectConfigName");
            var configPath = Path.Combine(dir, projectConfigName);

            if (File.Exists(configPath))
            {
                var projectInfo = await _projectBLL?.LoadProjectInfoFromConfigAsync(configPath);
                if (projectInfo != null)
                {
                    ProjectInfo = projectInfo;
                    _projectBLL.SetCurrentProjectInfo(ProjectInfo);
                    _eventAggregator.GetEvent<SelectedProjectInfoEvent>().Publish();
                    ExecuteName = _projectInfo.ProjectName + _projectInfo.ExecuteExtension;
                    _eventAggregator.GetEvent<ChangeMainViewEvent>().Publish(MainViewType.RunView);
                }
            }
        }

        private void ExecuteOpenFolderCommand()
        {
            _projectBLL?.OpenFolder();
        }
    }
}
