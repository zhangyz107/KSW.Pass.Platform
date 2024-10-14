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
using KSW.ATE01.Application.Helpers;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Domain.Projects.Entities;
using KSW.Helpers;
using Microsoft.Extensions.Logging;
using System.Configuration;
using System.Diagnostics;
using System.Windows;

namespace KSW.ATE01.Application.BLLs.Implements
{
    /// <summary>
    /// 项目业务逻辑层
    /// </summary>
    public class ProjectBLL : ServiceBase, IProjectBLL
    {
        private readonly IDialogService _dialogService;
        private ProjectInfoModel _currentProjectInfo;


        public ProjectBLL(
            IContainerProvider containerProvider,
            IDialogService dialogService) : base(containerProvider)
        {
            _dialogService = dialogService;
        }

        public async Task<bool> CreateProjectAsync(ProjectInfoModel projectInfo)
        {
            bool result = false;
            try
            {
                if (projectInfo.ProjectName.IsEmpty())
                    throw new ArgumentNullException(nameof(ProjectInfoModel.ProjectName));

                if (projectInfo.ProjectPath.IsEmpty())
                    throw new ArgumentNullException(nameof(ProjectInfoModel.ProjectPath));

                var templateName = ConfigurationManager.AppSettings["TemplateName"] ?? throw new ArgumentNullException("TemplateName");
                var templateDirName = ConfigurationManager.AppSettings["TemplateDirName"] ?? throw new ArgumentNullException("TemplateDirName");

                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var templatePath = Path.Combine(baseDirectory, templateDirName);

                if (Directory.Exists(projectInfo.ProjectPath) && MessageBox.Show($"当前路径下项目文件{projectInfo.ProjectName}已存在，是否进行覆盖", "项目已存在", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return result;

                CreateProjectByTemplate(projectInfo, templateName, templatePath);

                //补充项目信息
                ReplenishProjectInfo(projectInfo);

                //保存项目信息
                SaveProjectInfo(projectInfo);

                _currentProjectInfo = projectInfo;
                result = true;

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void RunProjecctByVS()
        {
            try
            {
                _currentProjectInfo.CheckNull(nameof(ProjectInfoModel));

                var setupInstance = SetupHelper.GetSetupInstance(false);
                string installationPath = setupInstance.GetInstallationPath();
                string executablePath = Path.Combine(installationPath, @"Common7\IDE\devenv.exe");

                if (!File.Exists(executablePath))
                    throw new FileNotFoundException("未能找到IDE!");

                var solutionPath = Path.Combine(_currentProjectInfo.ProjectPath, Path.GetFileName(_currentProjectInfo.ProjectName) + ".sln");
                if (!File.Exists(solutionPath))
                    throw new FileNotFoundException("未能找打项目文件!");

                Process.Start(executablePath, solutionPath);

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

        public List<ProjectInfoModel> ScanProjects(string folderName)
        {
            var result = new List<ProjectInfoModel>();
            try
            {
                folderName.CheckNull(nameof(folderName));

                if (!Directory.Exists(folderName))
                    throw new FileNotFoundException($"未能找到路径：{folderName}");

                var files = Directory.GetFiles(folderName, "*.atecfg", SearchOption.AllDirectories);
                if (files.IsEmpty())
                    return result;

                foreach (var file in files)
                {
                    var projectInfo = LoadProjectInfo(file);
                    if (projectInfo != null)
                        result.Add(projectInfo);
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ProjectInfoModel LoadProjectInfo(string file)
        {
            try
            {
                file.CheckNull(nameof(file));

                if (File.Exists(file))
                {
                    var projectInfo = XmlHelper.DeserializeFromXml<ProjectInfo>(file);
                    return projectInfo.MapTo<ProjectInfoModel>();
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetCurrentProjectInfo(ProjectInfoModel projectInfo)
        {
            if (projectInfo != null)
                _currentProjectInfo = projectInfo;
        }

        private void CreateProjectByTemplate(ProjectInfoModel projectInfo, string templateName, string templatePath)
        {
            var processBarParameters = ProcessBarHelper.CreateProcessBarParameters(async (action) =>
            {
                try
                {
                    var isInstalled = await ProjectTemplateHelper.IsTemplateInstalled(templateName);
                    if (!isInstalled)
                    {
                        var installedResult = await ProjectTemplateHelper.InstallTemplate(templatePath);
                        if (installedResult)
                            Log?.LogInformation("模板安装成功");
                        else
                            throw new Exception("模板安装失败");
                    }

                    var createResult = await ProjectTemplateHelper.CreateSolutionByTemplate(projectInfo.ProjectPath, templateName);
                    if (createResult)
                        Log?.LogInformation("项目创建成功");
                    else
                        throw new Exception("项目创建失败");
                }
                catch (Exception)
                {
                    throw;
                }
            });

            ProcessBarHelper.ShowProcessBarDialog(_dialogService, processBarParameters);
        }

        private void ReplenishProjectInfo(ProjectInfoModel projectInfo)
        {
            //完善项目相关信息
            projectInfo.CreateTime = DateTime.Now;
            projectInfo.ProjectVersion = new Version("1.0.0000.1").ToString();
        }

        private void SaveProjectInfo(ProjectInfoModel projectInfo)
        {
            var configPath = Path.Combine(projectInfo.ProjectPath, projectInfo.ProjectName + projectInfo.ConfigurationExtension);
            var entity = projectInfo.MapTo<ProjectInfo>();
            XmlHelper.SerializeToXml(entity, configPath);
        }
    }
}
