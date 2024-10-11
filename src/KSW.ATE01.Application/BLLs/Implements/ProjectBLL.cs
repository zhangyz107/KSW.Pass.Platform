/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：ProjectBLL.cs
// 功能描述：项目业务逻辑层
//
// 作者：zhangyingzhong
// 日期：2024/10/10 14:39
// 修改记录(Revision History)
//
//------------------------------------------------------------*/
using KSW.Application;
using KSW.ATE01.Application.BLLs.Abstractions;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Data;
using KSW.Localization;

namespace KSW.ATE01.Application.BLLs.Implements
{
    /// <summary>
    /// 项目业务逻辑层
    /// </summary>
    public class ProjectBLL : ServiceBase, IProjectBLL
    {
        private readonly IContainerProvider _containerProvider;
        private readonly ILanguageManager _languageManager;
        private ProjectInfoModel _currentProjectInfo;


        public ProjectBLL(
            IContainerProvider containerProvider) : base(containerProvider)
        {
            _containerProvider = containerProvider;

            _languageManager = _containerProvider.Resolve<ILanguageManager>();
        }

        public Task CreateProject(ProjectInfoModel projectInfo)
        {
            try
            {
                if (projectInfo.ProjectName.IsEmpty())
                    throw new ArgumentNullException(nameof(ProjectInfoModel.ProjectName));

                if (projectInfo.ProjectPath.IsEmpty())
                    throw new ArgumentNullException(nameof(ProjectInfoModel.ProjectPath));


                //todo:创建项目
                projectInfo.CreateTime = DateTime.Now;
                projectInfo.ProjectVersion = "1.0.0000.1";

                _currentProjectInfo = projectInfo;

                return Task.CompletedTask;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ProjectInfoModel GetCurrentProjectInfo()
        {
            return _currentProjectInfo;
        }
    }
}
