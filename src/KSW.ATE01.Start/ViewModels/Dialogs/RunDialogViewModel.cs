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
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Project.Base.Models;
using KSW.ATE01.Project.Base.Models.TestPlans;
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
        private readonly ITestPlanBLL _testPlanBLL;
        private ProjectInfoModel _projectInfo;
        private int _loopExecuted;
        private int _failCount;
        private TestPlanModel _testPlan;
        private ObservableCollection<FlowInfoModel> _flowList = new ObservableCollection<FlowInfoModel>();
        private ObservableCollection<SiteInfoModel> _siteList = new ObservableCollection<SiteInfoModel>();
        private RealTimeTxtView _realTimeTxtView;

        #endregion

        #region Properties
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

        /// <summary>
        /// 已执行循环
        /// </summary>
        public int LoopExecuted
        {
            get => _loopExecuted;
            set => SetProperty(ref _loopExecuted, value);
        }

        /// <summary>
        /// 失败数
        /// </summary>
        public int FailCount
        {
            get => _failCount;
            set => SetProperty(ref _failCount, value);
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
            _loadTestPlanCommand ?? (_loadTestPlanCommand = new DelegateCommand(ExecuteLoadTestPlanCommand));

        private DelegateCommand _setTestItemCommand;
        public DelegateCommand SetTestItemCommand =>
            _setTestItemCommand ?? (_setTestItemCommand = new DelegateCommand(ExecuteSetTestItemCommand));

        private DelegateCommand _startTestCommand;
        public DelegateCommand StartTestCommand =>
            _startTestCommand ?? (_startTestCommand = new DelegateCommand(ExecuteStartTestCommand));

        private DelegateCommand _endTestCommand;
        public DelegateCommand EndTestCommand =>
            _endTestCommand ?? (_endTestCommand = new DelegateCommand(ExecuteEndTestCommand));

        private DelegateCommand _oKCommand;
        public DelegateCommand OKCommand =>
            _oKCommand ?? (_oKCommand = new DelegateCommand(ExecuteOKCommand));

        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(ExecuteCancelCommand));

        public ProjectInfoModel ProjectInfo
        {
            get => _projectInfo;
            private set => SetProperty(ref _projectInfo, value);
        }
        #endregion

        public RunDialogViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
            _projectBLL = ContainerProvider?.Resolve<IProjectBLL>();
            _testPlanBLL = ContainerProvider?.Resolve<ITestPlanBLL>();
        }

        private void ExecuteLoadingCommand()
        {
            RealTimeTxtView = ContainerProvider.Resolve<RealTimeTxtView>();
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            if (_projectInfo != null && _projectBLL.SaveProjectInfo(_projectInfo))
            {
                _projectBLL.SetCurrentProjectInfo(_projectInfo);
                _eventAggregator.GetEvent<ProjectInfoUpdateEvent>().Publish();
            }
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
                _testPlan = await _testPlanBLL?.LoadTestPlanAsync(_projectInfo);
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

            }, async (e) => await DialogService.ShowMessageDialog(e.Message, MessageBoxButton.OK, MessageBoxImage.Warning));

        }

        private async void ExecuteSetTestItemCommand()
        {
            await ExecuteWithExceptionHandling(() =>
              {
                  var result = _testPlanBLL?.SetTestPlanFlow(FlowList, _projectInfo);
              }, async (e) => await DialogService.ShowMessageDialog(e.Message, MessageBoxButton.OK, MessageBoxImage.Warning));

        }

        private async void ExecuteStartTestCommand()
        {
            await ExecuteWithExceptionHandling(async () =>
            {
                //todo 先保证生成dll
                //if (await _projectBLL?.ReleaseSolutionAsync(_projectInfo))
                //{
                    CommonData.ProjectInfo = _projectInfo.MapTo<Project.Base.Models.Projects.ProjectInfo>();
                    CommonData.TestPlan = await _testPlanBLL.LoadTestPlanAsync(_projectInfo);

                    _projectBLL?.StartTestPlan(_projectInfo);

                //}

            }, async (e) => await DialogService.ShowMessageDialog(e.Message, MessageBoxButton.OK, MessageBoxImage.Warning));
        }

        private void ExecuteEndTestCommand()
        {

        }

        private void ExecuteOKCommand()
        {

            RaiseRequestClose(new DialogResult(ButtonResult.OK));
        }

        private void ExecuteCancelCommand()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
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
