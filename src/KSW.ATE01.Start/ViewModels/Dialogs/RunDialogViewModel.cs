/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：RunDialogViewModel.cs
// 功能描述：运行窗口视图模型
//
// 作者：zhangyingzhong
// 日期：2024/10/10 13:41
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.ATE01.Application.BLLs.Abstractions.Projects;
using KSW.ATE01.Application.Events.Projects;
using KSW.ATE01.Application.Managers.Abstractions.TestPlans;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Models;
using KSW.ATE01.Project.Base.Models.TestPlans;
using KSW.ATE01.Project.Base.Services.Loggers;
using KSW.ATE01.Project.Base.Services.Memory;
using KSW.ATE01.Start.Views;
using KSW.Helpers;
using KSW.Ui;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace KSW.ATE01.Start.ViewModels.Dialogs
{
    /// <summary>
    /// 运行窗口视图模型
    /// </summary>
    public class RunDialogViewModel : ViewModelBase, IDialogAware
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private readonly IProjectBLL _projectBLL;
        private readonly ITestPlanManager _testPlanBLL;
        private ProjectInfoModel _projectInfo;
        private TestPlanModel _testPlan;
        private ObservableCollection<FlowInfoModel> _flowList = new ObservableCollection<FlowInfoModel>();
        private ObservableCollection<SiteInfoModel> _siteList = new ObservableCollection<SiteInfoModel>();
        private RealTimeTxtView _realTimeTxtView;
        private bool _canLoadTestPlan = true;
        private bool _canExecuteSetTest = true;
        private bool _canExecuteStartTest = false;
        private bool _canExecuteEndTest = false;
        private bool _canExecuteLooping = false;
        private bool _canExecuteStop = false;
        private bool _isRunning = false;
        private bool _loopCountEnabled = true;
        private bool _loopDelayEnabled = true;
        #endregion

        #region Properties
        public ProjectInfoModel ProjectInfo
        {
            get => _projectInfo;
            private set => SetProperty(ref _projectInfo, value);
        }

        public DialogCloseListener RequestClose { get; }

        public string Title => L["Run"];

        public bool? IsAllItemsSelected
        {
            get
            {
                bool? result = false;
                var selected = _flowList?.Select(item => item.IsSelected)?.Distinct()?.ToList();
                if (selected != null)
                    result = selected.Count == 1 ? selected.Single() : (bool?)null;
                return result;
            }
            set
            {
                if (value.HasValue)
                {
                    SelectAllItems(value.Value);
                    RaisePropertyChanged();
                }
            }
        }

        public bool? IsAllSitesSelected
        {
            get
            {
                bool? result = false;
                var selected = _siteList?.Select(item => item.IsSelected)?.Distinct()?.ToList();
                if (selected != null)
                    result = selected.Count == 1 ? selected.Single() : (bool?)null;
                return result;
            }
            set
            {
                if (value.HasValue)
                {
                    SelectAllSites(value.Value);
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<FlowInfoModel> FlowList
        {
            get => _flowList;
            set => SetProperty(ref _flowList, value);
        }

        public ObservableCollection<SiteInfoModel> SiteList
        {
            get => _siteList;
            set => SetProperty(ref _siteList, value);
        }

        public RealTimeTxtView RealTimeTxtView
        {
            get => _realTimeTxtView;
            set => SetProperty(ref _realTimeTxtView, value);
        }

        public bool LoopCountEnabled
        {
            get => _loopCountEnabled;
            set => SetProperty(ref _loopCountEnabled, value);
        }

        public bool LoopDelayEnabled
        {
            get => _loopDelayEnabled;
            set => SetProperty(ref _loopDelayEnabled, value);
        }

        #endregion

        #region Command
        private DelegateCommand _loadingCommand;
        public DelegateCommand LoadingCommand =>
            _loadingCommand ?? (_loadingCommand = new DelegateCommand(ExecuteLoadingCommand));

        private DelegateCommand _openFolderCommand;
        public DelegateCommand OpenFolderCommand =>
            _openFolderCommand ?? (_openFolderCommand = new DelegateCommand(ExecuteOpenFolderCommand));

        private DelegateCommand _loadTestPlanCommand;
        public DelegateCommand LoadTestPlanCommand =>
            _loadTestPlanCommand ?? (_loadTestPlanCommand = new DelegateCommand(ExecuteLoadTestPlanCommand, () => { return _canLoadTestPlan; }));

        private AsyncDelegateCommand _loopingCommand;
        public AsyncDelegateCommand LoopingCommand => _loopingCommand ?? (_loopingCommand = new AsyncDelegateCommand(ExecuteLoopingCommand, () => { return _canExecuteLooping; }));

        private DelegateCommand _stopCommand;
        public DelegateCommand StopCommand => _stopCommand ?? (_stopCommand = new DelegateCommand(ExecuteStopCommand, () => { return _canExecuteStop; }));

        private DelegateCommand _setTestItemCommand;
        public DelegateCommand SetTestItemCommand =>
            _setTestItemCommand ?? (_setTestItemCommand = new DelegateCommand(ExecuteSetTestItemCommand, () => { return _flowList.Any() && _canExecuteSetTest; }));

        private AsyncDelegateCommand _startTestCommand;
        public AsyncDelegateCommand StartTestCommand =>
            _startTestCommand ?? (_startTestCommand = new AsyncDelegateCommand(ExecuteStartTestCommand, () => { return _canExecuteStartTest; }));

        private AsyncDelegateCommand _endTestCommand;
        public AsyncDelegateCommand EndTestCommand =>
            _endTestCommand ?? (_endTestCommand = new AsyncDelegateCommand(ExecuteEndTestCommand, () => { return _canExecuteEndTest; }));
        #endregion

        public RunDialogViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
            //_projectBLL = ContainerProvider?.Resolve<IProjectBLL>();
            _testPlanBLL = ContainerProvider?.Resolve<ITestPlanManager>();
        }

        private void ExecuteLoadingCommand()
        {
            RealTimeTxtView = ContainerProvider.Resolve<RealTimeTxtView>();
        }

        public bool CanCloseDialog()
        {
            return !_isRunning;
        }

        public async void OnDialogClosed()
        {
            var saveResult = await _projectBLL.UpdateAsync(_projectInfo);
            _eventAggregator.GetEvent<ProjectInfoUpdateEvent>().Publish();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            LoadData();
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose.Invoke(dialogResult);
        }

        private void LoadData()
        {
            var currentProjectInfo = _projectBLL?.GetCurrentProjectInfo();
            ProjectInfo = currentProjectInfo != null ? DeepCopy.Copy(currentProjectInfo) : null;
        }

        private void ExecuteOpenFolderCommand()
        {
            var folderDialog = new OpenFolderDialog()
            {
                Title = L["SelectFolder"],
            };

            if (!_projectInfo.DatalogPath.IsEmpty() && Directory.Exists(_projectInfo.DatalogPath))
                folderDialog.InitialDirectory = _projectInfo.DatalogPath;

            if (folderDialog.ShowDialog() == true)
            {
                var folderName = folderDialog.FolderName;
                _projectInfo.DatalogPath = folderName;
            }
        }

        private async void ExecuteLoadTestPlanCommand()
        {
            await ExecuteWithExceptionHandling(async () =>
            {
                _isRunning = true;
                _canLoadTestPlan = false;
                ChangeCommandsState();

                //_testPlan = await _testPlanBLL?.LoadTestPlanAsync(_projectInfo);

                ATE01ShareMemory.LoadedTestPlanFilePath = (Path.IsPathRooted(ATE01ShareMemory.TestPlanFilePath) ? ATE01ShareMemory.TestPlanFilePath : Path.Combine(Path.GetDirectoryName(_projectInfo.ProjectPath), ATE01ShareMemory.TestPlanFilePath));

                ShareMemoryInTestPlan<TestPlanModel> instance = ShareMemoryInTestPlan<TestPlanModel>.Instance;
                instance.Create();
                instance.WriteObject(_testPlan);

                if (_testPlan?.Flow?.IsEmpty() == false)
                {
                    FlowList.Clear();
                    foreach (var flow in _testPlan?.Flow)
                    {
                        var flowInfo = flow.MapTo<FlowInfoModel>();
                        flowInfo.PropertyChanged += (sender, args) =>
                        {
                            if (args.PropertyName.Equals(nameof(FlowModel.IsSelected)))
                            {
                                if (sender is FlowInfoModel model)
                                {
                                    model.Enable = model.IsSelected ? null : "False";
                                }
                                RaisePropertyChanged(nameof(IsAllItemsSelected));
                            }
                        };
                        FlowList.Add(flowInfo);
                    }
                    RaisePropertyChanged(nameof(IsAllItemsSelected));
                }

                if (_testPlan?.Channel?.IsEmpty() == false)
                {
                    SiteList.Clear();
                    foreach (var channel in _testPlan?.Channel)
                    {
                        foreach (var site in channel.Sites)
                        {
                            if (!SiteList.Any(x => site.SiteName.Equals(x.SiteName)))
                            {
                                var siteInfo = site.MapTo<SiteInfoModel>();
                                SiteList.Add(siteInfo);
                                siteInfo.PropertyChanged += (sender, args) =>
                                {
                                    if (args.PropertyName.Equals(nameof(SiteInfoModel.IsSelected)))
                                    {
                                        RaisePropertyChanged(nameof(IsAllSitesSelected));
                                    }
                                };
                            }
                        }
                    }
                    RaisePropertyChanged(nameof(IsAllSitesSelected));
                }

                _canExecuteStartTest = true;
                _canExecuteLooping = true;

            }, async (e) => await DialogService.ShowMessageDialog(e.Message, MessageBoxButton.OK, MessageBoxImage.Warning),
            () =>
            {
                _isRunning = false;
                _canLoadTestPlan = true;
                ChangeCommandsState();
            });

        }

        private void ChangeCommandsState()
        {
            LoadTestPlanCommand.RaiseCanExecuteChanged();
            SetTestItemCommand.RaiseCanExecuteChanged();
            StartTestCommand.RaiseCanExecuteChanged();
            EndTestCommand.RaiseCanExecuteChanged();
            LoopingCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();

        }

        private async void ExecuteSetTestItemCommand()
        {
            await ExecuteWithExceptionHandling(() =>
              {
                  //var result = _testPlanBLL?.SetTestPlanFlow(FlowList, _projectInfo);
              }, async (e) => await DialogService.ShowMessageDialog(e.Message, MessageBoxButton.OK, MessageBoxImage.Warning));

        }

        private async Task ExecuteStartTestCommand()
        {
            if (!_siteList.Any(x => x.IsSelected))
            {
                DialogService.ShowMessageDialog(L["NoSiteSelected"], MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await ExecuteWithExceptionHandling(async () =>
            {
                _isRunning = true;
                _canExecuteStartTest = false;
                _canLoadTestPlan = false;
                ChangeCommandsState();

                var processBarParameters = ProcessBarHelper.CreateProcessBarParameters(async (action) =>
                {
                    //todo 先保证生成dll
                    if (await _projectBLL?.ReleaseSolutionAsync(_projectInfo))
                    {
                        var commonData = CommonData.Instance;
                        var globalSetting = GlobalSetting.Instance;

                        PrintResultLog.PrintRealTimeTxt = _projectInfo.SaveRealTimeText == true;
                        if (commonData != null)
                        {
                            globalSetting.ProjectInfo = _projectInfo.MapTo<Project.Base.Models.Projects.ProjectInfo>();
                            commonData.TestPlan = await _testPlanBLL.LoadTestPlanAsync(_projectInfo);
                            commonData.UseSiteName = _siteList.Where(x => x.IsSelected).Select(x => x.SiteName).ToList();
                        }

                        await _projectBLL?.StartTestAsync(_flowList.ToList(), _projectInfo);

                    }
                });

                await ProcessBarHelper.ShowProcessBarDialogAsync(DialogService, processBarParameters);

                _canExecuteEndTest = true;
            },
            async (e) =>
            {
                await DialogService.ShowMessageDialog(e.Message, MessageBoxButton.OK, MessageBoxImage.Warning);
            },
            () =>
            {
                _canLoadTestPlan = true;
                _canExecuteStartTest = true;
                ChangeCommandsState();

                _isRunning = false;
            });
        }

        private async Task ExecuteEndTestCommand()
        {
            await ExecuteWithExceptionHandling(async () =>
            {
                _isRunning = true;
                _canExecuteEndTest = false;

                ChangeCommandsState();

                await _projectBLL?.EndTestAsync(_projectInfo);

                _canExecuteStartTest = true;
            },
            async (e) =>
            {
                await DialogService.ShowMessageDialog(e.Message, MessageBoxButton.OK, MessageBoxImage.Warning);
                _canExecuteEndTest = true;
            },
            () =>
            {
                _isRunning = false;
                ChangeCommandsState();
            });
        }

        private async Task ExecuteLoopingCommand()
        {
            if (_projectInfo.LoopCount == 0)
                return;

            if (!_siteList.Any(x => x.IsSelected))
            {
                await DialogService.ShowMessageDialog(L["NoSiteSelected"], MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _isRunning = true;
            _canLoadTestPlan = false;
            _canExecuteSetTest = false;
            _canExecuteStartTest = false;
            _canExecuteEndTest = false;
            _canExecuteLooping = false;
            _canExecuteStop = true;
            _loopCountEnabled = false;
            _loopDelayEnabled = false;
            _projectInfo.FailCount = 0;
            ChangeCommandsState();

            await ExecuteWithExceptionHandling(async () =>
            {
                await _projectBLL.ExecuteLoopingAsync(_flowList.ToList(), _projectInfo);

                _canExecuteEndTest = true;
            },
            async (e) => await DialogService.ShowMessageDialog(e.Message, MessageBoxButton.OK, MessageBoxImage.Warning)
            , () =>
            {
                _isRunning = false;
                _canLoadTestPlan = true;
                _canExecuteSetTest = true;
                _canExecuteStartTest = true;
                _canExecuteEndTest = true;
                _canExecuteLooping = true;
                _canExecuteStop = false;
                _loopCountEnabled = true;
                _loopDelayEnabled = true;

                ChangeCommandsState();
            });
        }

        private void ExecuteStopCommand()
        {
            _projectBLL.StopLooping();
        }

        private void SelectAllItems(bool select)
        {
            if (FlowList.IsEmpty())
                return;

            foreach (var flow in FlowList)
            {
                flow.IsSelected = select;
            }
        }

        private void SelectAllSites(bool select)
        {
            if (SiteList.IsEmpty())
                return;

            foreach (var flow in SiteList)
            {
                flow.IsSelected = select;
            }
        }
    }
}
