using KSW.Application;
using KSW.ATE01.Application.BLLs.Abstractions.Projects;
using KSW.ATE01.Application.Helpers;
using KSW.ATE01.Application.Managers.Abstractions.Projects;
using KSW.ATE01.Application.Managers.Abstractions.TestPlans;
using KSW.Exceptions;
using KSW.Helpers;
using System.Configuration;

namespace KSW.ATE01.Application.Managers.Implements.Projects
{
    public class ProjectManager : ServiceBase, IProjectManager
    {
        private readonly IProjectBLL _projectBLL;
        private readonly ITestPlanManager _testPlanBLL;
        private readonly string _csprojExt = ".csproj";
        private readonly string _releaseDirName = "Release";
        private readonly string _slnExt = ".sln";

        public ProjectManager(IContainerProvider containerProvider) : base(containerProvider)
        {
            _projectBLL = ContainerProvider.IsRegistered<IProjectBLL>() ? ContainerProvider?.Resolve<IProjectBLL>() : null;
            _testPlanBLL = ContainerProvider.IsRegistered<ITestPlanManager>() ? ContainerProvider?.Resolve<ITestPlanManager>() : null;
        }

        public async Task<bool> SaveAsProjectInfoAsync(string saveAsDir, string saveAsName)
        {
            var result = false;
            try
            {
                var templateName = ConfigurationManager.AppSettings["TemplateName"] ?? throw new ArgumentNullException("TemplateName");
                var templateDirName = ConfigurationManager.AppSettings["TemplateDirName"] ?? throw new ArgumentNullException("TemplateDirName");
                var testPlanDirName = ConfigurationManager.AppSettings["TestPlanDirName"] ?? throw new ArgumentNullException("TemplateDirName");

                var currentProjectInfo = _projectBLL?.GetCurrentProjectInfo();
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
                var newProjectInfo = DeepCopy.Copy(currentProjectInfo);
                newProjectInfo.Id = null;
                newProjectInfo.ProjectName = saveAsName;
                newProjectInfo.ProjectPath = targetDir;
                newProjectInfo.ReleasePath = Path.Combine(targetDir, _releaseDirName);
                _projectBLL?.CreateAsync(newProjectInfo);
                #endregion

                _projectBLL?.SetCurrentProjectInfo(newProjectInfo);

                result = true;
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }
    }
}
