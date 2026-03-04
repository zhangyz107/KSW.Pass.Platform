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
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Events.Projects;
using KSW.ATE01.Application.Events.TestPlans;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Project.Base.Models;
using KSW.ATE01.Project.Base.Services.Loggers;
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
        private readonly IPinOverviewBLL _pinOverviewBLL;
        private readonly IProjectBLL _projectBLL;
        private readonly ITestItemInfoBLL _testItemInfoBLL;
        private readonly ISiteInfoBLL _siteInfoBLL;
        private ProjectInfoModel _projectInfo;
        private ObservableCollection<TestItemInfoModel> _testItemList = new ObservableCollection<TestItemInfoModel>();
        private ObservableCollection<SiteInfoModel> _siteList = new ObservableCollection<SiteInfoModel>();
        private RealTimeTxtView _realTimeTxtView;
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
                var selected = _testItemList?.Select(item => item.Enable)?.Distinct()?.ToList();
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

        public ObservableCollection<TestItemInfoModel> TestItemList
        {
            get => _testItemList;
            set => SetProperty(ref _testItemList, value);
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

        private DelegateCommand<TestItemInfoModel> _arrowUpCommand;
        public DelegateCommand<TestItemInfoModel> ArrowUpCommand =>
            _arrowUpCommand ?? (_arrowUpCommand = new DelegateCommand<TestItemInfoModel>(ExecuteArrowUpCommand, (testItem) => _testItemList.IndexOf(testItem) != 0));

        private DelegateCommand<TestItemInfoModel> _arrowDownCommand;
        public DelegateCommand<TestItemInfoModel> ArrowDownCommand =>
            _arrowDownCommand ?? (_arrowDownCommand = new DelegateCommand<TestItemInfoModel>(ExecuteArrowDownCommand, (testItem) => _testItemList.IndexOf(testItem) != _testItemList.Count - 1));

        private AsyncDelegateCommand _loopingCommand;
        public AsyncDelegateCommand LoopingCommand => _loopingCommand ?? (_loopingCommand = new AsyncDelegateCommand(ExecuteLoopingCommand, () => { return _canExecuteLooping; }));

        private DelegateCommand _stopCommand;
        public DelegateCommand StopCommand => _stopCommand ?? (_stopCommand = new DelegateCommand(ExecuteStopCommand, () => { return _canExecuteStop; }));

        private DelegateCommand _setTestItemCommand;
        public DelegateCommand SetTestItemCommand =>
            _setTestItemCommand ?? (_setTestItemCommand = new DelegateCommand(ExecuteSetTestItemCommand, () => { return _testItemList.Any() && _canExecuteSetTest; }));

        private AsyncDelegateCommand _startTestCommand;
        public AsyncDelegateCommand StartTestCommand =>
            _startTestCommand ?? (_startTestCommand = new AsyncDelegateCommand(ExecuteStartTestCommand, () => { return _canExecuteStartTest; }));

        private AsyncDelegateCommand _endTestCommand;
        public AsyncDelegateCommand EndTestCommand =>
            _endTestCommand ?? (_endTestCommand = new AsyncDelegateCommand(ExecuteEndTestCommand, () => { return _canExecuteEndTest; }));
        #endregion

        public RunDialogViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator,
            IPinOverviewBLL pinOverviewBLL,
            IProjectBLL projectBLL,
            ITestItemInfoBLL testItemInfoBLL,
            ISiteInfoBLL siteInfoBLL) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
            _pinOverviewBLL = pinOverviewBLL;
            _projectBLL = projectBLL;
            _testItemInfoBLL = testItemInfoBLL;
            _siteInfoBLL = siteInfoBLL;

            InitEvent();
        }

        private void InitEvent()
        {
            _eventAggregator.GetEvent<SelectedProjectInfoEvent>().Subscribe(SelectedProjectInfo);
            _eventAggregator.GetEvent<UpdateTestPlanEvent>().Subscribe(SelectedProjectInfo);
        }

        private void SelectedProjectInfo()
        {
            LoadData();
        }

        private void ExecuteLoadingCommand()
        {
            RealTimeTxtView = ContainerProvider.Resolve<RealTimeTxtView>();
            LoadData();
        }

        public bool CanCloseDialog()
        {
            return !_isRunning;
        }

        public async void OnDialogClosed()
        {
            var saveResult = await _projectBLL.UpdateAsync(_projectInfo);
            _eventAggregator.GetEvent<UpdateProjectInfoEvent>().Publish();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose.Invoke(dialogResult);
        }

        private async void LoadData()
        {
            var currentProjectInfo = _projectBLL?.GetCurrentProjectInfo();
            ProjectInfo = currentProjectInfo;
            if (_projectInfo != null)
            {
                var pinOverview = await _pinOverviewBLL.GetPinOverviewFromProjectIdAsync(_projectInfo.Id);

                _testItemList.Clear();
                var testItemList = await _testItemInfoBLL?.GetListByProjectIdAsync(_projectInfo?.Id);
                if (!testItemList.IsEmpty())
                {
                    var flowTestItemList = testItemList.Where(x => x.FlowIndex != null).ToList();
                    if (flowTestItemList.Any())
                    {
                        _testItemList.AddRange(flowTestItemList.OrderBy(x => x.FlowIndex));
                        var unflowTestItemList = testItemList.Where(x => x.FlowIndex == null).ToList();
                        _testItemList.AddRange(unflowTestItemList.OrderBy(x => x.SortId));
                    }
                    else
                        _testItemList.AddRange(testItemList.OrderBy(x => x.SortId));

                    foreach (var testItem in testItemList)
                    {
                        testItem.ArrowUpCommand = ArrowUpCommand;
                        testItem.ArrowDownCommand = ArrowDownCommand;
                        testItem.PropertyChanged += (sender, args) =>
                        {
                            if (args.PropertyName.Equals(nameof(TestItemInfoModel.Enable)))
                            {
                                RaisePropertyChanged(nameof(IsAllItemsSelected));
                            }
                        };

                    }
                    ArrowUpCommand.RaiseCanExecuteChanged();
                    ArrowDownCommand.RaiseCanExecuteChanged();
                    RaisePropertyChanged(nameof(IsAllItemsSelected));
                }

                _siteList.Clear();
                var siteList = await _siteInfoBLL?.GetSiteInfosFromPinOverviewId(pinOverview?.Id);
                if (!siteList.IsEmpty())
                {
                    _siteList.AddRange(siteList);
                    foreach (var site in siteList)
                    {
                        site.PropertyChanged += (sender, args) =>
                        {
                            if (args.PropertyName.Equals(nameof(SiteInfoModel.IsSelected)))
                            {
                                RaisePropertyChanged(nameof(IsAllSitesSelected));
                            }
                        };
                    }

                    RaisePropertyChanged(nameof(IsAllSitesSelected));
                }

                _canExecuteLooping = true;
                _canExecuteStartTest = true;
                _canExecuteSetTest = true;
            }
            ChangeCommandsState();
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

        private void ExecuteArrowUpCommand(TestItemInfoModel testItem)
        {
            var oldIndex = _testItemList.IndexOf(testItem);
            _testItemList.Move(oldIndex, oldIndex - 1);

            ArrowUpCommand.RaiseCanExecuteChanged();
            ArrowDownCommand.RaiseCanExecuteChanged();
        }

        private void ExecuteArrowDownCommand(TestItemInfoModel testItem)
        {
            var oldIndex = _testItemList.IndexOf(testItem);
            _testItemList.Move(oldIndex, oldIndex + 1);

            ArrowUpCommand.RaiseCanExecuteChanged();
            ArrowDownCommand.RaiseCanExecuteChanged();
        }

        private void ChangeCommandsState()
        {
            SetTestItemCommand.RaiseCanExecuteChanged();
            StartTestCommand.RaiseCanExecuteChanged();
            EndTestCommand.RaiseCanExecuteChanged();
            LoopingCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();

        }

        private async void ExecuteSetTestItemCommand()
        {
            await ExecuteWithExceptionHandling(async () =>
            {
                await SaveDataAsync();

            }, async (e) => await DialogService.ShowMessageDialog(e.Message, MessageBoxButton.OK, MessageBoxImage.Warning));

        }

        private async Task SaveDataAsync()
        {
            // 保存项目信息
            await _projectBLL?.UpdateAsync(_projectInfo);

            var index = 0;
            foreach (var testItem in _testItemList)
                testItem.FlowIndex = index++;
            // 保存测试项信息
            await _testItemInfoBLL?.SaveAsync(_testItemList.ToList());

            // 保存站点信息
            await _siteInfoBLL?.SaveAsync(_siteList.ToList());
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
                ChangeCommandsState();

                var processBarParameters = ProcessBarHelper.CreateProcessBarParameters(async (action) =>
                {
                    // 保存数据
                    await SaveDataAsync();

                    //todo 先保证生成dll
                    if (await _projectBLL?.ReleaseSolutionAsync(_projectInfo))
                    {
                        var commonData = CommonData.Instance;
                        var globalSetting = GlobalSetting.Instance;

                        PrintResultLog.PrintRealTimeTxt = _projectInfo.SaveRealTimeText == true;
                        if (commonData != null)
                        {
                            globalSetting.ProjectInfo = _projectInfo.MapTo<Project.Base.Models.Projects.ProjectInfo>();
                            commonData.TestPlan = await _projectBLL?.ConversionTestPlanAsync(_projectInfo?.Id);

                            commonData.UseSiteName = _siteList.Where(x => x.IsSelected).Select(x => x.SiteName).ToList();
                        }

                        await _projectBLL?.StartTestAsync(_projectInfo);

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
                // 保存数据
                await SaveDataAsync();

                //todo 先保证生成dll
                if (await _projectBLL?.ReleaseSolutionAsync(_projectInfo))
                {
                    var commonData = CommonData.Instance;
                    var globalSetting = GlobalSetting.Instance;

                    PrintResultLog.PrintRealTimeTxt = _projectInfo.SaveRealTimeText == true;
                    if (commonData != null)
                    {
                        globalSetting.ProjectInfo = _projectInfo.MapTo<Project.Base.Models.Projects.ProjectInfo>();
                        commonData.TestPlan = await _projectBLL?.ConversionTestPlanAsync(_projectInfo?.Id);

                        commonData.UseSiteName = _siteList.Where(x => x.IsSelected).Select(x => x.SiteName).ToList();
                    }

                    await _projectBLL.ExecuteLoopingAsync(_projectInfo);

                }
            },
            async (e) => await DialogService.ShowMessageDialog(e.Message, MessageBoxButton.OK, MessageBoxImage.Warning)
            , () =>
            {
                _isRunning = false;
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
            if (_testItemList.IsEmpty())
                return;

            foreach (var testItem in _testItemList)
            {
                testItem.Enable = select;
            }
        }

        private void SelectAllSites(bool select)
        {
            if (_siteList.IsEmpty())
                return;

            foreach (var site in _siteList)
            {
                site.IsSelected = select;
            }
        }
    }
}
