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
using KSW.Ui;

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
        private readonly IProjectBLL _projectBLL;
        private ProjectInfoModel _projectInfo;
        private string _testPlanName;
        private string _executeName;
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
        #endregion

        public ProjectDetailViewModel(
            IContainerExtension containerProvider,
            IEventAggregator eventAggregator) : base(containerProvider)
        {
            _containerProvider = containerProvider;
            _eventAggregator = eventAggregator;

            _projectBLL = _containerProvider.Resolve<IProjectBLL>();

            RegisterEvent();
        }

        private void RegisterEvent()
        {
            _eventAggregator.GetEvent<ProjectInfoUpdateEvent>().Subscribe(ProjectInfoUpdate);
        }

        private void ProjectInfoUpdate()
        {
            ProjectInfo = _projectBLL.GetCurrentProjectInfo();
            TestPlanName = _projectInfo.ProjectName + _projectInfo.TestPlanExtension;
            ExecuteName = _projectInfo.ProjectName + _projectInfo.ExecuteExtension;
        }
    }
}
