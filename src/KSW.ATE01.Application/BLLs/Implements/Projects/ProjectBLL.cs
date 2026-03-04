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
using KSW.ATE01.Application.Managers.Abstractions.Projects;
using KSW.ATE01.Application.Managers.Abstractions.TestPlans;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Data;
using KSW.ATE01.Domain.Projects.Core.Enums;
using KSW.ATE01.Domain.Projects.Entities;
using KSW.ATE01.Domain.Projects.Repositories;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Results;
using KSW.ATE01.Project.Base.Enums.Errors;
using KSW.ATE01.Project.Base.Enums.Results;
using KSW.ATE01.Project.Base.Models;
using KSW.ATE01.Project.Base.Models.Errors;
using KSW.ATE01.Project.Base.Models.Exceptions;
using KSW.ATE01.Project.Base.Models.TestPlans;
using KSW.ATE01.Project.Base.Services.Loggers;
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
    public class ProjectBLL : CrudServiceBase<ProjectInfo>, IProjectBLL
    {
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IProjectInfoRepository _repository;
        private readonly IProjectManager _projectManager;
        private readonly ITestPlanManager _testPlanManager;
        private ProjectInfoModel _currentProjectInfo;
        private readonly string _logDirName = "Log";
        private readonly string _releaseDirName = "Release";
        private readonly string _csprojExt = ".csproj";
        private readonly string _slnExt = ".sln";
        private readonly bool _isDeleteProjectDir;
        private readonly Stopwatch _stopwatch;
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
            ISystemUnitOfWork unitOfWork,
            IProjectInfoRepository repository,
            IDialogService dialogService,
            IEventAggregator eventAggregator,
            IProjectManager projectManager,
            ITestPlanManager testPlanManager) : base(containerProvider, unitOfWork, repository)
        {
            _repository = repository;
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;
            _projectManager = projectManager;
            _testPlanManager = testPlanManager;

            bool.TryParse(ConfigurationManager.AppSettings["IsDeleteProjectDir"], out bool flag);
            _isDeleteProjectDir = flag;
            _stopwatch = new Stopwatch();
        }


        public async Task<ProjectInfoModel> GetByIdAsync(string id)
        {
            var entity = await _repository.FindByIdAsync(id);
            return entity?.MapTo<ProjectInfoModel>();
        }

        public async Task<ProjectInfoModel> LoadProjectInfoFromConfigAsync(string configPath)
        {
            if (File.Exists(configPath))
            {
                using (var fs = File.Open(configPath, FileMode.Open, FileAccess.Read))
                {
                    var buffer = new byte[16];
                    var count = fs.Read(buffer);
                    var projectId = new Guid(buffer);

                    var entity = await _repository.FindByIdAsync(projectId);
                    return entity?.MapTo<ProjectInfoModel>();
                }
            }
            return null;
        }

        public async Task<List<ProjectInfoModel>> GetListAsync()
        {
            var list = await _repository.FindAllAsync();
            return list.MapToList<ProjectInfoModel>();
        }

        public async Task<string> CreateAsync(ProjectInfoModel projectInfo)
        {
            try
            {
                var templateName = ConfigurationManager.AppSettings["TemplateName"] ?? throw new ArgumentNullException("TemplateName");
                var templateDirName = ConfigurationManager.AppSettings["TemplateDirName"] ?? throw new ArgumentNullException("TemplateDirName");
                var projectConfigName = ConfigurationManager.AppSettings["ProjectConfigName"] ?? throw new ArgumentNullException("ProjectConfigName");

                var isExist = await _repository.ExistsAsync(x => x.ProjectName.Equals(projectInfo.ProjectName));
                if (isExist)
                    throw new Warning(string.Format($"{L["FieldAlreadyExists"]}", projectInfo.ProjectName));

                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var templatePath = Path.Combine(baseDirectory, templateDirName);

                if (!Directory.Exists(projectInfo.ProjectPath))
                    Directory.CreateDirectory(projectInfo.ProjectPath);
                else if (Directory.Exists(projectInfo.ProjectPath) && (await _dialogService.ShowMessageDialog($"当前路径下项目文件{projectInfo.ProjectName}已存在，是否进行覆盖", MessageBoxButton.YesNo, MessageBoxImage.Question))?.Result == ButtonResult.No)
                    return string.Empty;

                await CreateProjectByTemplate(projectInfo, templateName, templatePath);

                //补充项目信息
                ReplenishProjectInfo(projectInfo);

                //保存项目信息
                var entity = projectInfo.MapTo<ProjectInfo>();
                if (projectInfo.Id.IsEmpty())
                {
                    entity.Init();
                    await CreateAsync(entity);

                    var projectFileName = Path.Combine(projectInfo?.ProjectPath, projectConfigName);
                    using (var fs = File.Open(projectFileName, FileMode.Create, FileAccess.Write))
                    {
                        var guidArray = entity.Id.ToByteArray();
                        fs.Write(guidArray);
                        fs.Flush();
                    }
                }

                _currentProjectInfo = await GetByIdAsync(entity.Id.SafeString());

                return entity.Id.SafeString();
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
                string installationPath = string.Empty;
                var vsPair = SetupHelper.GetAllAndLatestPath();
                if (!vsPair.latestPath.IsEmpty() && Directory.Exists(vsPair.latestPath))
                {
                    installationPath = vsPair.latestPath;
                }
                else
                {
                    var setupInstance = SetupHelper.GetSetupInstance(false);
                    installationPath = setupInstance.GetInstallationPath();
                }

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

        public virtual ProjectInfoModel GetCurrentProjectInfo()
        {
            return _currentProjectInfo;
        }

        public async Task<ProjectInfoModel> UpdateAsync(ProjectInfoModel projectInfo)
        {
            var entity = projectInfo.MapTo<ProjectInfo>();
            await UpdateAsync(projectInfo.Id, entity);
            _currentProjectInfo = await GetByIdAsync(projectInfo.Id);
            return _currentProjectInfo;
        }

        public void SetCurrentProjectInfo(ProjectInfoModel projectInfo)
        {
            //if (projectInfo != null)
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

                await UpdateAsync(projectInfo);

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

        public async Task<bool> CopyAsync(string saveAsDir, string saveAsName, string version, ProjectInfoModel projectInfo = null)
        {
            var result = false;
            try
            {
                projectInfo = projectInfo ?? _currentProjectInfo;
                if (projectInfo == null)
                    throw new Warning(string.Format("{0}{1}", L["ProjectFile"], L["IsEmpty"]));

                var projectId = await _projectManager.SaveAsProjectInfoAsync(projectInfo?.Id, saveAsDir, saveAsName, version);
                await _testPlanManager.CopyTestPlanByProjectIdAsync(projectInfo.Id, projectId);
                await CommitAsync();

                result = true;
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        public async Task<TestPlanModel> ConversionTestPlanAsync(string projectId)
        {
            return await _testPlanManager?.ConversionTestPlanAsync(projectId);
        }

        public async Task StartTestAsync(ProjectInfoModel projectInfo = null)
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
                        GlobalSetting.Instance.StartTestTime = DateTime.Now;
                        _stopwatch.Restart();
                        //运行TestStart
                        var flag = await ExecuteFunctionAsync(ProcessStage.TestStart, instance, classType, startTestMethod, null);
                        _alreadyStartLot = true;

                        if (flag)   //运行FlowStart
                        {
                            var result = await ExecuteTestItemsInFlowAsync(projectInfo, instance, classType);
                            flag = result.Item1;
                        }
                        //if (flag)   //运行TestEnd
                        //    flag = ExecuteFunction(ProcessStage.TestEnd, instance, classType, endTestMethod, null);
                    }
                    else
                    {
                        Message.StatusClear();
                        //运行FlowStart
                        var flag = await ExecuteTestItemsInFlowAsync(projectInfo, instance, classType);
                    }

                }
                // 释放加载的上下文和程序集
                loadContext.Unload();

                // 在适当的地方调用GC以释放未管理的资源
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                ErrorMessages.InsGeneral.MarkerError(nameof(StartTestAsync), new object[] { ex.Message });
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

                    var flag = ExecuteFunctionAsync(ProcessStage.TestEnd, instance, classType, endTestMethod, null);

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

        public async Task ExecuteLoopingAsync(ProjectInfoModel projectInfo = null)
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
                        var flag = await ExecuteFunctionAsync(ProcessStage.TestStart, instance, classType, startTestMethod, null);
                        _alreadyStartLot = true;
                    }
                    else
                    {
                        Message.StatusClear();
                    }

                    await LoopTest(instance, classType, projectInfo, token);
                    //await Task.Factory.StartNew(async () => await LoopTest(instance, classType, projectInfo, token), token);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task LoopTest(object instance, Type? classType, ProjectInfoModel projectInfo, CancellationToken token)
        {
            try
            {
                if (projectInfo.LoopExecuted >= _loopTargeCount)
                {
                    _loopTargeCount = projectInfo.LoopExecuted + projectInfo.LoopCount ?? 0;
                }

                while (projectInfo.LoopExecuted < _loopTargeCount)
                {

                    //运行FlowStart
                    var result = await ExecuteTestItemsInFlowAsync(projectInfo, instance, classType);
                    projectInfo.FailCount += result.Item2;
                    var failFlag = projectInfo.StopOnFail == true && result.Item2 > 0;
                    projectInfo.LoopExecuted++;

                    if (token.IsCancellationRequested || failFlag)
                        break;

                    await Task.Delay((projectInfo.DelayBetweenLoops != null && projectInfo.DelayBetweenLoops>0) ? (int)projectInfo.DelayBetweenLoops * 1000 : 0);
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

                var createResult = await ProjectTemplateHelper.CreateSolutionByTemplateAsync(projectInfo.ProjectPath, templateName);
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

        private async Task<bool> ExecuteFunctionAsync(ProcessStage stage, object? classInstance, Type classType, string methodName, object?[]? parameters)
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

                var obj = await Task.Factory.StartNew(() =>
                {
                    return method.Invoke(classInstance, parameters);
                });

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
                            ErrorMessages.InsGeneral.MarkerError(nameof(ExecuteFunctionAsync), new object[]
                            {
                                ate.Message
                            });
                        }
                        else
                        {
                            FlowStatus = FlowStatus.ExecuteTestItemFail;
                            ErrorMessages.Flow.InternalError(innerException, nameof(ExecuteFunctionAsync));
                            ErrorMessages.Flow.DefaultFunctionExecutionError(nameof(ExecuteFunctionAsync), new object[]
                            {
                                methodName,
                                innerException.Message,
                            });
                        }
                        break;
                    case ProcessStage.TestStart:
                        FlowStatus = FlowStatus.TestStartExecuteFailed;
                        ErrorMessages.InsGeneral.MarkerLog(nameof(ExecuteFunctionAsync), new object[]
                        {
                            innerException.Message,
                        });

                        ErrorMessages.Flow.FailToExecuteForceHalt(nameof(ExecuteFunctionAsync), new object[]
                        {
                            methodName,
                            "\n" + innerException.Message,
                        });
                        break;
                    case ProcessStage.TestEnd:
                        FlowStatus = FlowStatus.TestEndExecuteFailed;
                        ErrorMessages.InsGeneral.MarkerLog(nameof(ExecuteFunctionAsync), new object[]
                        {
                            innerException.Message,
                        });

                        ErrorMessages.Flow.FailToExecuteForceHalt(nameof(ExecuteFunctionAsync), new object[]
                        {
                            methodName,
                            "\n" + innerException.Message,
                        });
                        break;
                    case ProcessStage.FlowStart:
                        FlowStatus = FlowStatus.FlowStartExecuteFailed;
                        ErrorMessages.InsGeneral.MarkerLog(nameof(ExecuteFunctionAsync), new object[]
                        {
                            innerException.Message,
                        });

                        ErrorMessages.Flow.DefaultFunctionExecutionError(nameof(ExecuteFunctionAsync), new object[]
                        {
                            methodName,
                            innerException.Message,
                        });
                        break;
                    case ProcessStage.FlowEnd:
                        FlowStatus = FlowStatus.FlowEndExecuteFailed;
                        ErrorMessages.InsGeneral.MarkerLog(nameof(ExecuteFunctionAsync), new object[]
                        {
                           innerException.Message,
                        });

                        ErrorMessages.Flow.DefaultFunctionExecutionError(nameof(ExecuteFunctionAsync), new object[]
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

        private async Task<(bool, int)> ExecuteTestItemsInFlowAsync(ProjectInfoModel projectInfo, object? instance, Type classType)
        {
            var result = true;
            var failCount = 0;
            var spendTime = 0L;
            var startFlowMethod = ConfigurationManager.AppSettings["StartFlowMethod"] ?? throw new ArgumentNullException("StartFlowMethod");
            var endFlowMethod = ConfigurationManager.AppSettings["EndFlowMethod"] ?? throw new ArgumentNullException("EndFlowMethod");
            var commonData = CommonData.Instance;
            var testPlan = commonData?.TestPlan;
            _stopwatch.Restart();
            try
            {
                result = await ExecuteFunctionAsync(ProcessStage.FlowStart, instance, classType, startFlowMethod, null);
                //var testItems =
                if (projectInfo.IsPrintTime == true)
                {
                    var message = $"====== Flow Start time : {_stopwatch.ElapsedMilliseconds - spendTime} ms ====== ";
                    spendTime = _stopwatch.ElapsedMilliseconds;
                    PrintResultLog.Message(message);
                }

                var flowIds = testPlan?.Flow?.Where(x => x.IsSelected).OrderBy(x => x.SortId).Select(x => x.TestItemId);
                var testItems = new List<TestItemModel>();
                foreach (var flowId in flowIds)
                {
                    var testItem = testPlan?.TestItem?.FirstOrDefault(x => x.Id.ToGuid().Equals(flowId));
                    testItems.Add(testItem);
                }

                if (!testItems.IsEmpty())
                {
                    foreach (var testItem in testItems)
                    {
                        SetCommonData(testItem);
                        result = await ExecuteFunctionAsync(ProcessStage.TestItem, instance, classType, testItem.FunctionName, null);
                        if (!result)
                            ++failCount;
                    }
                }

                if (projectInfo.IsPrintTime == true)
                {
                    var message = $"====== The whole flow time : {_stopwatch.ElapsedMilliseconds - spendTime} ms ====== ";
                    spendTime = _stopwatch.ElapsedMilliseconds;
                    Result.TestTime = spendTime;
                }

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                result = await ExecuteFunctionAsync(ProcessStage.FlowEnd, instance, classType, endFlowMethod, null);
                _stopwatch.Stop();
                if (projectInfo.IsPrintTime == true)
                {
                    var message = $"====== Flow End time : {_stopwatch.ElapsedMilliseconds - spendTime} ms ====== ";
                    spendTime = _stopwatch.ElapsedMilliseconds;
                }
            }

            return (result, failCount);
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

        public async Task DeleteAsync(string id)
        {
            if (id.IsEmpty())
                return;

            var projectInfo = await _repository.FindByIdAsync(id);
            await _repository?.RemoveAsync(id);
            await _testPlanManager?.DeleteTestPlanByProjectIdAsync(id);
            await CommitAsync();

            if (_isDeleteProjectDir && Directory.Exists(projectInfo?.ProjectPath))
            {
                Directory.Delete(projectInfo?.ProjectPath, true);
            }
        }

        public void OpenFolder(ProjectInfoModel projectInfo = null)
        {
            projectInfo = projectInfo ?? _currentProjectInfo;
            if (projectInfo == null || projectInfo.ProjectPath.IsEmpty() || !Directory.Exists(projectInfo.ProjectPath))
                return;
            Process.Start("explorer.exe", projectInfo.ProjectPath);
        }

        public async Task ImportTestPlanAsync(string filePath, ProjectInfoModel projectInfo = null)
        {
            if (File.Exists(filePath))
            {
                projectInfo = projectInfo ?? _currentProjectInfo;
                await _testPlanManager.ImportTestPlanAsync(filePath, projectInfo?.Id);

                await CommitAsync();
            }
        }

        public async Task ExportTestPlanAsync(string filePath, ProjectInfoModel projectInfo = null)
        {
            projectInfo = projectInfo ?? _currentProjectInfo;
            await _testPlanManager.ExportTestPlanAsync(filePath, projectInfo.Id);
        }
    }
}
