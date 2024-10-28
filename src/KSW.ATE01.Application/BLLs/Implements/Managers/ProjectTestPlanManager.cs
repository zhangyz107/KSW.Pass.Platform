using KSW.Application;
using KSW.ATE01.Application.BLLs.Abstractions.Managers;
using KSW.ATE01.Application.BLLs.Abstractions.Projects;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Helpers;
using KSW.ATE01.Domain.Projects.Core.Enums;
using KSW.ATE01.Domain.Projects.Entities;
using KSW.Exceptions;
using KSW.Helpers;
using System.Configuration;
using System.Xml;

namespace KSW.ATE01.Application.BLLs.Implements.Managers
{
    public class ProjectTestPlanManager : ServiceBase, IProjectTestPlanManager
    {
        private readonly IProjectBLL _projectBLL;
        private readonly ITestPlanBLL _testPlanBLL;
        private readonly string _csprojExt = ".csproj";
        private readonly string _releaseDirName = "Release";
        private readonly string _slnExt = ".sln";

        public ProjectTestPlanManager(IContainerProvider containerProvider) : base(containerProvider)
        {
            _projectBLL = ContainerProvider.IsRegistered<IProjectBLL>() ? ContainerProvider?.Resolve<IProjectBLL>() : null;
            _testPlanBLL = ContainerProvider.IsRegistered<ITestPlanBLL>() ? ContainerProvider?.Resolve<ITestPlanBLL>() : null;
        }

        public async Task<bool> SaveAsProjectInfoAsync(TestPlanType testPlanType, string saveAsDir, string saveAsName)
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
                newProjectInfo.ProjectName = saveAsName;
                newProjectInfo.ProjectPath = targetDir;
                newProjectInfo.TestPlanType = testPlanType;
                newProjectInfo.ReleasePath = Path.Combine(targetDir, _releaseDirName);
                newProjectInfo.CreateTime = DateTime.Now;
                _projectBLL?.SaveProjectInfo(newProjectInfo);
                #endregion

                #region 处理测试计划类型变更

                if (currentProjectInfo.TestPlanType != testPlanType)
                {
                    ChangeTestPlanType(testPlanType, targetDir, saveAsName);

                    //另存为TestPlan类型值
                    var testPlan = await _testPlanBLL?.LoadTestPlanAsync(currentProjectInfo);

                    //拷贝测试计划
                    if (await _projectBLL?.CopyTestPlanAsync(newProjectInfo))
                    {
                        _testPlanBLL?.SaveAsTestPlan(testPlan, testPlanType, targetDir, saveAsName);
                    }

                    var csvDirPath = Path.Combine(targetDir, testPlanDirName);
                    if (testPlanType == TestPlanType.Excel && Directory.Exists(csvDirPath))
                        Directory.Delete(csvDirPath, true);
                }
                else
                {
                    switch (newProjectInfo.TestPlanType)
                    {
                        case TestPlanType.Excel:
                            if (!Directory.Exists(newProjectInfo.ReleasePath))
                                Directory.CreateDirectory(newProjectInfo.ReleasePath);
                            File.Copy(Path.Combine(currentProjectInfo.ReleasePath, currentProjectInfo.ProjectName + currentProjectInfo.TestPlanExtension), Path.Combine(newProjectInfo.ReleasePath, newProjectInfo.ProjectName + newProjectInfo.TestPlanExtension));
                            break;
                        case TestPlanType.Csv:
                            break;
                    }
                }

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


        private void ChangeTestPlanType(TestPlanType targetType, string targetDir, string projectName)
        {
            var projectPath = Path.Combine(targetDir, projectName + _csprojExt);
            // 加载 XML 文档
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(projectPath);

            var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("msbuild", "http://schemas.microsoft.com/developer/msbuild/2003");

            // 查找 TestplanType 节点
            XmlNode testplanTypeNode = xmlDoc.SelectSingleNode("/msbuild:Project/msbuild:PropertyGroup/msbuild:TestplanType", nsmgr);

            if (testplanTypeNode != null)
            {
                // 修改 TestplanType 的值
                testplanTypeNode.InnerText = targetType.Description();
                Console.WriteLine("TestplanType value updated to: " + testplanTypeNode.InnerText);

                // 保存修改后的 XML 文件
                xmlDoc.Save(projectPath);
                Console.WriteLine("XML file saved.");
            }
            else
            {
                Console.WriteLine("TestplanType node not found.");
            }
        }

    }
}
