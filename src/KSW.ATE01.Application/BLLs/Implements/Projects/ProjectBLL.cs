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
using KSW.ATE01.Application.BLLs.Abstractions.Projects;
using KSW.ATE01.Application.Events.Projects;
using KSW.ATE01.Application.Helpers;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Domain.Projects.Core.Enums;
using KSW.ATE01.Domain.Projects.Entities;
using KSW.ATE01.Project.Base.Enums.Errors;
using KSW.ATE01.Project.Base.Enums.Results;
using KSW.ATE01.Project.Base.Models;
using KSW.ATE01.Project.Base.Models.Errors;
using KSW.ATE01.Project.Base.Models.Exceptions;
using KSW.ATE01.Project.Base.Models.TestPlans;
using KSW.Exceptions;
using KSW.Helpers;
using KSW.Reflections;
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
        private readonly IEventAggregator _eventAggregator;
        private ProjectInfoModel _currentProjectInfo;
        private readonly string _logDirName = "Log";
        private readonly string _releaseDirName = "Release";
        private readonly string _csprojExt = ".csproj";
        private readonly string _slnExt = ".sln";
        private readonly string _excelExtension;
        private bool _alreadyStartLot = false;
        private List<string> _errorMessageList = new List<string>();
        private FlowStatus _flowStatus;
        private CancellationTokenSource _tokenSource;
        private int _loopTargeCount = 0;
        #region Properties
        public FlowStatus FlowStatus
        {
            get { return _flowStatus; }
            set { _flowStatus = value; }
        }
        #endregion

        public ProjectBLL(
            IContainerProvider containerProvider,
            IDialogService dialogService,
            IEventAggregator eventAggregator) : base(containerProvider)
        {
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;
            _excelExtension = ConfigurationManager.AppSettings["ExcelExtension"];
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

                var process = new Process();
                process.StartInfo.FileName = executablePath;
                process.StartInfo.Arguments = solutionPath;
                process.Start();

                _eventAggregator.GetEvent<ShellRevealControlEvent>().Publish(false);

                // 监视进程退出
                process.EnableRaisingEvents = true;
                process.Exited += (sender, e) =>
                {
                    _eventAggregator.GetEvent<ShellRevealControlEvent>().Publish(true);
                };
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
                KSW.Helpers.XmlHelper.SerializeToXml(entity, configPath);
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
                    var projectInfo = KSW.Helpers.XmlHelper.DeserializeFromXml<ProjectInfo>(file);
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
        public async Task StartTestAsync(List<FlowInfoModel> flows, ProjectInfoModel projectInfo = null)
        {
            try
            {
                projectInfo = projectInfo ?? _currentProjectInfo;

                if (projectInfo == null)
                    throw new Warning(string.Format("{0}{1}", L["ProjectFile"], L["IsEmpty"]));

                var testItemName = ConfigurationManager.AppSettings["TestItemName"] ?? throw new ArgumentNullException("TestItemName");
                var startTestMethod = ConfigurationManager.AppSettings["StartTestMethod"] ?? throw new ArgumentNullException("StartTestMethod");
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var dllPath = Path.Combine(projectInfo.ReleasePath, projectInfo.ProjectName + projectInfo.ExecuteExtension);

                var loadContext = new PluginLoadContext(Path.GetDirectoryName(dllPath), assemblies);
                var assem = loadContext.LoadFromAssemblyPath(dllPath);
                var classType = assem.GetType(testItemName);
                // 获取实现该接口的类型
                if (classType != null)
                {
                    // 创建类的实例
                    object instance = Activator.CreateInstance(classType);
                    if (!_alreadyStartLot)
                    {
                        Message.InitializeStatusClear();
                        Message.StatusClear();
                        //运行TestStart
                        var flag = ExecuteFunction(ProcessStage.TestStart, instance, classType, startTestMethod, null);
                        _alreadyStartLot = true;

                        if (flag)   //运行FlowStart
                            flag = ExecuteTestItemsInFlow(instance, classType, flows);

                        //if (flag)   //运行TestEnd
                        //    flag = ExecuteFunction(ProcessStage.TestEnd, instance, classType, endTestMethod, null);
                    }
                    else
                    {
                        Message.StatusClear();
                        //运行FlowStart
                        var flag = ExecuteTestItemsInFlow(instance, classType, flows);
                    }

                }
                // 释放加载的上下文和程序集
                loadContext.Unload();

                // 在适当的地方调用GC以释放未管理的资源
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task EndTestAsync(ProjectInfoModel projectInfo = null)
        {
            try
            {
                projectInfo = projectInfo ?? _currentProjectInfo;

                if (projectInfo == null)
                    throw new Warning(string.Format("{0}{1}", L["ProjectFile"], L["IsEmpty"]));

                var testItemName = ConfigurationManager.AppSettings["TestItemName"] ?? throw new ArgumentNullException("TestItemName");
                var endTestMethod = ConfigurationManager.AppSettings["EndTestMethod"] ?? throw new ArgumentNullException("EndTestMethod");
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var dllPath = Path.Combine(projectInfo.ReleasePath, projectInfo.ProjectName + projectInfo.ExecuteExtension);

                var loadContext = new PluginLoadContext(Path.GetDirectoryName(dllPath), assemblies);
                var assem = loadContext.LoadFromAssemblyPath(dllPath);
                var classType = assem.GetType(testItemName);

                // 获取实现该接口的类型
                if (classType != null)
                {
                    // 创建类的实例
                    object instance = Activator.CreateInstance(classType);

                    var flag = ExecuteFunction(ProcessStage.TestEnd, instance, classType, endTestMethod, null);

                    // 释放加载的上下文和程序集
                    loadContext.Unload();

                    // 在适当的地方调用GC以释放未管理的资源
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
            catch (Exception ex)
            {
                ErrorMessages.InsGeneral.MarkerError(nameof(EndTestAsync), new object[] { ex.Message });
                throw;
            }
            finally
            {
                _alreadyStartLot = false;
            }
        }

        public async Task ExecuteLoopingAsync(List<FlowInfoModel> flows, ProjectInfoModel projectInfo = null)
        {
            try
            {
                projectInfo = projectInfo ?? _currentProjectInfo;

                if (projectInfo == null)
                    throw new Warning(string.Format("{0}{1}", L["ProjectFile"], L["IsEmpty"]));

                var testItemName = ConfigurationManager.AppSettings["TestItemName"] ?? throw new ArgumentNullException("TestItemName");
                var startTestMethod = ConfigurationManager.AppSettings["StartTestMethod"] ?? throw new ArgumentNullException("StartTestMethod");
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var dllPath = Path.Combine(projectInfo.ReleasePath, projectInfo.ProjectName + projectInfo.ExecuteExtension);

                var loadContext = new PluginLoadContext(Path.GetDirectoryName(dllPath), assemblies);
                var assem = loadContext.LoadFromAssemblyPath(dllPath);
                var classType = assem.GetType(testItemName);


                // 获取实现该接口的类型
                if (classType != null)
                {
                    // 创建类的实例
                    object instance = Activator.CreateInstance(classType);
                    _tokenSource = new CancellationTokenSource();
                    var token = _tokenSource.Token;

                    if (!_alreadyStartLot)
                    {
                        Message.InitializeStatusClear();
                        Message.StatusClear();
                        _loopTargeCount = 0;
                        projectInfo.LoopExecuted = 0;
                        projectInfo.FailCount = 0;

                        //运行TestStart
                        var flag = ExecuteFunction(ProcessStage.TestStart, instance, classType, startTestMethod, null);
                        _alreadyStartLot = true;
                    }
                    else
                    {
                        Message.StatusClear();
                    }

                    await Task.Factory.StartNew(async () => await LoopTest(instance, classType, flows, projectInfo, token), token);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task LoopTest(object instance, Type? classType, List<FlowInfoModel> flows, ProjectInfoModel projectInfo, CancellationToken token)
        {
            try
            {
                if (projectInfo.LoopExecuted >= _loopTargeCount)
                {
                    _loopTargeCount = projectInfo.LoopExecuted + projectInfo.LoopCount;
                }

                while (projectInfo.LoopExecuted < _loopTargeCount)
                {
                    if (token.IsCancellationRequested)
                        break;

                    //运行FlowStart
                    var flag = ExecuteTestItemsInFlow(instance, classType, flows);

                    projectInfo.LoopExecuted++;

                    await Task.Delay(projectInfo.DelayBetweenLoops * 1000);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void StopLooping()
        {
            try
            {
                if (_tokenSource != null && !_tokenSource.IsCancellationRequested)
                    _tokenSource.Cancel();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private bool GetFlowStatus(bool returnValue)
        {
            bool result = false;
            var location = nameof(GetFlowStatus);
            try
            {
                if (returnValue && Message.ErrorStatus == ErrorStatus.Normal)
                    result = true;
                else
                {
                    if (Message.ErrorStatus != ErrorStatus.Normal)
                    {
                        _errorMessageList.Add(Message.GetErrorMessageAndClear());
                    }
                }
            }
            catch (Exception inner)
            {
                result = false;
                ErrorMessages.Flow.InternalError(inner, location);
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
            var suffix = $"*{_excelExtension}";
            var excelFiles = Directory.GetFiles(testPlanPath, suffix);
            if (!excelFiles.IsEmpty())
                foreach (var file in excelFiles)
                {
                    var targetPath = Path.Combine(projectInfo.ReleasePath, projectInfo.ProjectName + _excelExtension);
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

        private bool ExecuteFunction(ProcessStage stage, object? classInstance, Type classType, string methodName, object?[]? parameters)
        {
            var result = false;
            if (classInstance == null)
                return result;

            if (methodName.IsEmpty())
                return result;

            try
            {
                var method = classType.GetMethod(methodName);
                // 调用接口方法
                if (method == null)
                    return result;

                var obj = method.Invoke(classInstance, parameters);

                if (obj != null && obj.GetType().IsEnum)
                    result = (Test)obj == Test.Pass;

                var flag = GetFlowStatus(result);
                switch (stage)
                {
                    case ProcessStage.TestItem:
                        if (!flag)
                            FlowStatus = FlowStatus.ExecuteTestItemFail;
                        break;
                    case ProcessStage.TestStart:
                        FlowStatus = flag ? FlowStatus.TestStartExecuteSucceed : FlowStatus.TestStartExecuteFailed;
                        break;
                    case ProcessStage.TestEnd:
                        FlowStatus = flag ? FlowStatus.TestEndExecuteSucceed : FlowStatus.FlowEndExecuteFailed;
                        break;
                    case ProcessStage.FlowStart:
                        FlowStatus = flag ? FlowStatus.FlowStartExecuteSucceed : FlowStatus.FlowStartExecuteFailed;
                        break;
                    case ProcessStage.FlowEnd:
                        FlowStatus = flag ? FlowStatus.FlowEndExecuteSucceed : FlowStatus.FlowEndExecuteFailed;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                var innerException = GetInnerException(ex);
                switch (stage)
                {
                    case ProcessStage.TestItem:
                        if (innerException is ATEException ate)
                        {
                            ErrorMessages.InsGeneral.MarkerError(nameof(ExecuteFunction), new object[]
                            {
                                ate.Message
                            });
                        }
                        else
                        {
                            FlowStatus = FlowStatus.ExecuteTestItemFail;
                            ErrorMessages.Flow.InternalError(innerException, nameof(ExecuteFunction));
                            ErrorMessages.Flow.DefaultFunctionExecutionError(nameof(ExecuteFunction), new object[]
                            {
                                methodName,
                                innerException.Message,
                            });
                        }
                        break;
                    case ProcessStage.TestStart:
                        FlowStatus = FlowStatus.TestStartExecuteFailed;
                        ErrorMessages.InsGeneral.MarkerLog(nameof(ExecuteFunction), new object[]
                        {
                            innerException.Message,
                        });

                        ErrorMessages.Flow.FailToExecuteForceHalt(nameof(ExecuteFunction), new object[]
                        {
                            methodName,
                            "\n" + innerException.Message,
                        });
                        break;
                    case ProcessStage.TestEnd:
                        FlowStatus = FlowStatus.TestEndExecuteFailed;
                        ErrorMessages.InsGeneral.MarkerLog(nameof(ExecuteFunction), new object[]
                        {
                            innerException.Message,
                        });

                        ErrorMessages.Flow.FailToExecuteForceHalt(nameof(ExecuteFunction), new object[]
                        {
                            methodName,
                            "\n" + innerException.Message,
                        });
                        break;
                    case ProcessStage.FlowStart:
                        FlowStatus = FlowStatus.FlowStartExecuteFailed;
                        ErrorMessages.InsGeneral.MarkerLog(nameof(ExecuteFunction), new object[]
                        {
                            innerException.Message,
                        });

                        ErrorMessages.Flow.DefaultFunctionExecutionError(nameof(ExecuteFunction), new object[]
                        {
                            methodName,
                            innerException.Message,
                        });
                        break;
                    case ProcessStage.FlowEnd:
                        FlowStatus = FlowStatus.FlowEndExecuteFailed;
                        ErrorMessages.InsGeneral.MarkerLog(nameof(ExecuteFunction), new object[]
                        {
                           innerException.Message,
                        });

                        ErrorMessages.Flow.DefaultFunctionExecutionError(nameof(ExecuteFunction), new object[]
                        {
                            methodName,
                            innerException.Message,
                        });
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        private Exception GetInnerException(Exception ex)
        {
            while (ex.InnerException != null)
            {
                return GetInnerException(ex.InnerException);
            }

            return ex;
        }

        private bool ExecuteTestItemsInFlow(object? instance, Type classType, List<FlowInfoModel> flows)
        {
            var result = false;

            var startFlowMethod = ConfigurationManager.AppSettings["StartFlowMethod"] ?? throw new ArgumentNullException("StartFlowMethod");
            var endFlowMethod = ConfigurationManager.AppSettings["EndFlowMethod"] ?? throw new ArgumentNullException("EndFlowMethod");
            var commonData = CommonData.Instance;
            var testPlan = commonData?.TestPlan;
            try
            {
                result = ExecuteFunction(ProcessStage.FlowStart, instance, classType, startFlowMethod, null);
                var testItemNames = flows.Where(x => x.Enable.IsEmpty()).Select(x => x.TestItemName);
                var flowIds = testPlan.Flow.Where(x => testItemNames.Contains(x.TestItemName)).Select(x => x.TestItemId);
                var testItems = testPlan.TestItem.Where(x => flowIds.Contains(x.Id.ToGuid())).Select(x => x);
                foreach (var testItem in testItems)
                {
                    SetCommonData(testItem);
                    result = ExecuteFunction(ProcessStage.TestItem, instance, classType, testItem.FunctionName, null);
                }

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                result = ExecuteFunction(ProcessStage.FlowEnd, instance, classType, endFlowMethod, null);
            }

            return result;
        }

        private void SetCommonData(TestItemModel testItem)
        {
            var commonData = CommonData.Instance;
            if (commonData != null)
            {
                var limits = commonData.TestPlan.Limits;

                commonData.FunctionName = testItem.FunctionName;
                commonData.TestItemName = testItem.TestItemName;
                commonData.Force = System.Convert.ToDouble(testItem.Force);
                commonData.Pins = testItem.Pins;
                commonData.Level = testItem.Level;
                commonData.Timing = testItem.Timing;
                commonData.TestItemArgs = testItem.Args;
                commonData.TestItemLimit = limits.IsEmpty() ? null : limits.FirstOrDefault(x => x.TestItemId.Equals(testItem.Id.ToGuid()));
            }
        }
    }
}
