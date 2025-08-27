using KSW.Application;
using KSW.ATE01.Application.Helpers;
using KSW.ATE01.Application.Managers.Abstractions.Projects;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Domain.Projects.Entities;
using KSW.ATE01.Domain.Projects.Repositories;
using KSW.Exceptions;
using NPOI.Util;
using System.Configuration;

namespace KSW.ATE01.Application.Managers.Implements.Projects
{
    public class ProjectManager : ServiceBase, IProjectManager
    {
        private readonly IProjectInfoRepository _projectInfoRepository;
        private readonly string _releaseDirName = "Release";
        private readonly string _slnExt = ".sln";

        public ProjectManager(
            IContainerProvider containerProvider,
            IProjectInfoRepository projectInfoRepository) : base(containerProvider)
        {
            _projectInfoRepository = projectInfoRepository;
        }

        public async Task<string> SaveAsProjectInfoAsync(string projectId, string saveAsDir, string saveAsName, string version)
        {
            try
            {
                var templateName = ConfigurationManager.AppSettings["TemplateName"] ?? throw new ArgumentNullException("TemplateName");
                var templateDirName = ConfigurationManager.AppSettings["TemplateDirName"] ?? throw new ArgumentNullException("TemplateDirName");
                var testPlanDirName = ConfigurationManager.AppSettings["TestPlanDirName"] ?? throw new ArgumentNullException("TemplateDirName");

                var currentProjectInfo = await _projectInfoRepository?.FindByIdAsync(projectId);
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var templatePath = Path.Combine(baseDirectory, templateDirName);
                if (currentProjectInfo == null)
                    throw new Warning(string.Format("{0}{1}", L["ProjectFile"], L["IsEmpty"]));
                var targetDir = Path.Combine(saveAsDir, saveAsName);
                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);
                if (!await ProjectTemplateHelper.CopyProjectAsync(currentProjectInfo?.ProjectPath, targetDir))
                    throw new Warning(L["FileCopyFailed"]);

                #region 处理解决方案名及命名空间
                var oldSln = Path.Combine(targetDir, currentProjectInfo.ProjectName + _slnExt);
                await VSHelper.RenameSolutionAndProjctAsync(oldSln, saveAsName, currentProjectInfo.ProjectName, saveAsName);
                #endregion

                #region 保存项目配置
                var newProjectInfo = currentProjectInfo.Copy();
                var projectInfoModel = newProjectInfo.MapTo<ProjectInfoModel>();
                projectInfoModel.Id = null;
                projectInfoModel.ProjectName = saveAsName;
                projectInfoModel.ProjectPath = targetDir;
                projectInfoModel.ProjectVersion = version;
                projectInfoModel.ReleasePath = Path.Combine(targetDir, _releaseDirName);
                var projectInfo = projectInfoModel.MapTo<ProjectInfo>();
                projectInfo.Init();
                await _projectInfoRepository?.AddAsync(projectInfo);
                #endregion

                return projectInfo.Id.SafeString();
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
