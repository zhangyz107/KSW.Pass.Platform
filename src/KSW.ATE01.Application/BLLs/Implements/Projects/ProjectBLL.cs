﻿/*--------------------------------------------------------------
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
using KSW.ATE01.Application.BLLs.Abstractions.Projects;
using KSW.ATE01.Application.Helpers;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Domain.Projects.Core.Enums;
using KSW.ATE01.Domain.Projects.Entities;
using KSW.Exceptions;
using KSW.Helpers;
using Microsoft.Extensions.Logging;
using System.Configuration;
using System.Diagnostics;
using System.Windows;

namespace KSW.ATE01.Application.BLLs.Implements.Projects
{
    /// <summary>
    /// 项目业务逻辑层
    /// </summary>
    public class ProjectBLL : ServiceBase, IProjectBLL
    {
        private readonly IDialogService _dialogService;
        private ProjectInfoModel _currentProjectInfo;
        private readonly string _logDirName = "Log";
        private readonly string _releaseDirName = "Release";
        private readonly string _csprojExt = ".csproj";
        private readonly string _slnExt = ".sln";

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
                var templateName = ConfigurationManager.AppSettings["TemplateName"] ?? throw new ArgumentNullException("TemplateName");
                var templateDirName = ConfigurationManager.AppSettings["TemplateDirName"] ?? throw new ArgumentNullException("TemplateDirName");

                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var templatePath = Path.Combine(baseDirectory, templateDirName);

                if (!Directory.Exists(projectInfo.ProjectPath))
                    Directory.CreateDirectory(projectInfo.ProjectPath);
                else if (Directory.Exists(projectInfo.ProjectPath) && (await _dialogService.ShowMessageDialog($"当前路径下项目文件{projectInfo.ProjectName}已存在，是否进行覆盖", MessageBoxButton.YesNo, MessageBoxImage.Question))?.Result == ButtonResult.No)
                    return result;

                await CreateProjectByTemplate(projectInfo, templateName, templatePath);

                //补充项目信息
                ReplenishProjectInfo(projectInfo);

                //保存项目信息
                SaveProjectInfo(projectInfo);

                _currentProjectInfo = projectInfo;
                result = true;

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
                    throw new Warning(string.Format("{0}{1}", L["NotFound"], "IDE"));

                var solutionPath = Path.Combine(_currentProjectInfo.ProjectPath, Path.GetFileName(_currentProjectInfo.ProjectName) + ".sln");
                if (!File.Exists(solutionPath))
                    throw new Warning(string.Format("{0}{1}", L["NotFound"], L["ProjectFile"]));

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
                    throw new Warning(string.Format("{0}{1}:{2}", L["NotFound"], L["SelectFolder"], folderName));

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

        public bool SaveProjectInfo(ProjectInfoModel projectInfo)
        {
            var result = false;
            try
            {
                var configPath = Path.Combine(projectInfo.ProjectPath, projectInfo.ProjectName + projectInfo.ConfigurationExtension);
                var entity = projectInfo.MapTo<ProjectInfo>();
                XmlHelper.SerializeToXml(entity, configPath);
                result = true;
            }
            catch (Exception)
            {

                throw;
            }
            return result;
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

        public async Task<bool> SaveAsProjectInfoAsync(TestPlanType testPlanType, string saveAsDir, string saveAsName)
        {
            var result = false;
            try
            {
                #region 拷贝项目文件
                if (_currentProjectInfo == null)
                    throw new Warning(string.Format("{0}{1}", L["ProjectFile"], L["IsEmpty"]));
                var targetDir = Path.Combine(saveAsDir, saveAsName);
                if (!await ProjectTemplateHelper.CopyProjectAsync(_currentProjectInfo?.ProjectPath, targetDir))
                    throw new Warning(L["FileCopyFailed"]);
                #endregion

                #region 处理解决方案名及命名空间
                var oldSln = Path.Combine(targetDir, _currentProjectInfo.ProjectName + _slnExt);
                await VSHelper.RenameSolutionAndProjctAsync(oldSln, saveAsName, _currentProjectInfo.ProjectName, saveAsName);
                #endregion

                #region 处理测试计划类型变更

                #endregion

                #region 保存项目配置
                var newProjectInfo = DeepCopy.Copy(_currentProjectInfo);
                newProjectInfo.ProjectName = saveAsName;
                newProjectInfo.ProjectPath = targetDir;
                newProjectInfo.TestPlanType = testPlanType;
                newProjectInfo.CreateTime = DateTime.Now;
                SaveProjectInfo(newProjectInfo);

                _currentProjectInfo = newProjectInfo;
                #endregion

                result = true;
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        public async Task<bool> ReleaseSolutionAsync(ProjectInfoModel projectInfo = null, bool openReleaseDir = false)
        {
            var result = false;
            try
            {
                projectInfo = projectInfo ?? _currentProjectInfo;

                if (projectInfo == null)
                    throw new Warning(string.Format("{0}{1}", L["ProjectFile"], L["IsEmpty"]));

                var slnPath = Path.Combine(projectInfo.ProjectPath, projectInfo.ProjectName + _slnExt);
                if (!await ProjectTemplateHelper.ReleaseProjectAsync(slnPath, projectInfo.ReleasePath))
                    throw new Warning(L["PublishFailed"]);

                //打开发布文件夹
                if (openReleaseDir)
                    Process.Start("explorer.exe", projectInfo.ReleasePath);
                result = true;
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        public async Task<bool> CopyTestPlanAsync(ProjectInfoModel projectInfo = null)
        {
            var result = false;
            try
            {
                projectInfo = projectInfo ?? _currentProjectInfo;
                if (projectInfo == null)
                    throw new Warning(string.Format("{0}{1}", L["ProjectFile"], L["IsEmpty"]));

                switch (projectInfo.TestPlanType)
                {
                    case TestPlanType.Excel:
                        CopyExcelFile(projectInfo);
                        break;
                    case TestPlanType.Csv:
                        break;
                }

                result = true;
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        private void CopyExcelFile(ProjectInfoModel projectInfo)
        {
            var testPlanDirName = ConfigurationManager.AppSettings["TestPlanDirName"] ?? throw new ArgumentNullException("TemplateDirName");

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var testPlanPath = Path.Combine(baseDirectory, testPlanDirName);

            if (!Directory.Exists(testPlanPath))
                throw new Warning("");

            var excelFiles = Directory.GetFiles(testPlanPath, "*.xlsx");
            if (!excelFiles.IsEmpty())
                foreach (var file in excelFiles)
                {
                    var targetPath = Path.Combine(projectInfo.ReleasePath, projectInfo.ProjectName + ".xlsx");
                    if (!Directory.Exists(projectInfo.ReleasePath))
                        Directory.CreateDirectory(projectInfo.ReleasePath);
                    File.Copy(file, targetPath);
                }
        }

        private async Task CreateProjectByTemplate(ProjectInfoModel projectInfo, string templateName, string templatePath)
        {
            try
            {
                var isInstalled = await ProjectTemplateHelper.IsTemplateInstalledAsync(templateName);
                if (!isInstalled)
                {
                    var installedResult = await ProjectTemplateHelper.InstallTemplateAsync(templatePath);
                    if (installedResult)
                        Log?.LogInformation(L["TemplateInstalledSuccessfully"]);
                    else
                        throw new Warning(L["TemplateInstalledFailed"]);
                }

                var createResult = await ProjectTemplateHelper.CreateSolutionByTemplateAsync(projectInfo.TestPlanType, projectInfo.ProjectPath, templateName);
                if (createResult)
                    Log?.LogInformation(L["ProjectCreatedSuccessfully"]);
                else
                    throw new Warning(L["ProjectCreatedFailed"]);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void ReplenishProjectInfo(ProjectInfoModel projectInfo)
        {
            //完善项目相关信息
            projectInfo.CreateTime = DateTime.Now;
            projectInfo.ProjectVersion = new Version("1.0.0000.1").ToString();
            projectInfo.DatalogPath = Path.Combine(projectInfo.ProjectPath, _logDirName);
            projectInfo.ReleasePath = Path.Combine(projectInfo.ProjectPath, _releaseDirName);
        }

        private void FileRename(string srcFile, string destFile)
        {
            try
            {
                if (!File.Exists(srcFile))
                    throw new Warning(string.Format("{0}{1}:{2}", L["NotFound"], L["ProjectFile"], srcFile));

                File.Move(srcFile, destFile);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
