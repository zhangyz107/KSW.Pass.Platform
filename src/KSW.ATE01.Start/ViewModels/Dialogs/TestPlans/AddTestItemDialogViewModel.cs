
/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：AddTestItemDialogViewModel.cs
// 功能描述：添加测试项对话框视图模型
//
// 作者：zhangyingzhong
// 日期：2025/08/21 15:37
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.ATE01.Application.BLLs.Abstractions.Projects;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Events;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.Ui;
using KSW.UI.WPF.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace KSW.ATE01.Start.ViewModels.Dialogs.TestPlans
{
    /// <summary>
    /// 添加测试项对话框视图模型
    /// </summary>
    public class AddTestItemDialogViewModel : ViewModelBase, IDialogAware
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private readonly IProjectBLL _projectBLL;
        private readonly IPinOverviewBLL _pinOverviewBLL;
        private readonly IGroupInfoBLL _groupInfoBLL;
        private readonly IPinInfoBLL _pinInfoBLL;
        private readonly ITestItemInfoBLL _testItemInfoBLL;
        private readonly ILimitsBLL _limitsBLL;
        private readonly ILevelGroupBLL _levelGroupBLL;
        private readonly ITimingGroupBLL _timingGroupBLL;
        private ProjectInfoModel _projectInfo;
        private TestItemInfoModel _testItem;
        private ObservableCollection<AdditionalParameters> _parameterList = new ObservableCollection<AdditionalParameters>();
        private ICollectionView _filteredItems;
        private string _editString;
        #endregion

        #region Properties
        public DialogCloseListener RequestClose { get; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// 测试项
        /// </summary>
        public TestItemInfoModel TestItem
        {
            get => _testItem;
            set => SetProperty(ref _testItem, value);
        }

        /// <summary>
        /// 附加参数
        /// </summary>
        public ObservableCollection<AdditionalParameters> ParameterList
        {
            get => _parameterList;
            set => SetProperty(ref _parameterList, value);
        }

        /// <summary>
        /// 引脚或组下拉字典
        /// </summary>
        public Dictionary<Guid, string> GroupOrPinDic { get; private set; } = new Dictionary<Guid, string>();

        /// <summary>
        /// 过滤后的数据源
        /// </summary>
        public ICollectionView FilteredItems
        {
            get => _filteredItems;
            set => SetProperty(ref _filteredItems, value);
        }

        /// <summary>
        /// 编辑字符串
        /// </summary>
        public string EditString
        {
            get => _editString;
            set
            {
                if (SetProperty(ref _editString, value))
                {
                    FilteredItems.Refresh();
                }
            }
        }

        /// <summary>
        /// 门限字典
        /// </summary>
        public Dictionary<Guid, string> LimitsDic { get; private set; } = new Dictionary<Guid, string>();

        /// <summary>
        /// 电平组字典
        /// </summary>
        public Dictionary<Guid, string> LevelGroupDic { get; private set; } = new Dictionary<Guid, string>();

        /// <summary>
        /// 时钟组字典
        /// </summary>
        public Dictionary<Guid, string> TimingGroupDic { get; private set; } = new Dictionary<Guid, string>();
        #endregion

        #region Commands
        private DelegateCommand _loadingCommand;
        public DelegateCommand LoadingCommand =>
            _loadingCommand ?? (_loadingCommand = new DelegateCommand(ExecuteLoadingCommand));

        /// <summary>
        /// 添加附加参数命令
        /// </summary>
        private DelegateCommand _addCommand;
        public DelegateCommand AddCommand =>
            _addCommand ?? (_addCommand = new DelegateCommand(ExecuteAddCommand));

        /// <summary>
        /// 删除命令
        /// </summary>
        private DelegateCommand<AdditionalParameters> _deleteCommand;
        public DelegateCommand<AdditionalParameters> DeleteCommand =>
            _deleteCommand ?? (_deleteCommand = new DelegateCommand<AdditionalParameters>(ExecuteDeleteCommand));

        /// <summary>
        /// 确定命令
        /// </summary>
        private AsyncDelegateCommand _oKCommand;
        public AsyncDelegateCommand OKCommand =>
            _oKCommand ?? (_oKCommand = new AsyncDelegateCommand(ExecuteOKCommand, CheckInputValue));

        /// <summary>
        /// 取消命令
        /// </summary>
        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(ExecuteCancelCommand));
        #endregion

        public AddTestItemDialogViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator,
            IProjectBLL projectBLL,
            IPinOverviewBLL pinOverviewBLL,
            IGroupInfoBLL groupInfoBLL,
            IPinInfoBLL pinInfoBLL,
            ITestItemInfoBLL testItemInfoBLL,
            ILimitsBLL limitsBLL,
            ILevelGroupBLL levelGroupBLL,
            ITimingGroupBLL timingGroupBLL
            ) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
            _pinOverviewBLL = pinOverviewBLL;
            _projectBLL = projectBLL;
            _groupInfoBLL = groupInfoBLL;
            _pinInfoBLL = pinInfoBLL;
            _testItemInfoBLL = testItemInfoBLL;
            _limitsBLL = limitsBLL;
            _levelGroupBLL = levelGroupBLL;
            _timingGroupBLL = timingGroupBLL;
        }

        private void ExecuteLoadingCommand()
        {
            FilteredItems = CollectionViewSource.GetDefaultView(GroupOrPinDic);
            FilteredItems.Filter = FilterItems;
        }

        private bool FilterItems(object obj)
        {
            if (_editString.IsEmpty())
                return true;

            if (obj is KeyValuePair<Guid, string> pair)
            {
                return pair.Value.IndexOf(_editString, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            else
                return false;

        }

        private bool CheckInputValue()
        {
            var result = true;

            result &= !_testItem.GroupOrPinId.IsEmpty();
            result &= !_testItem.TestItemName.IsEmpty();
            result &= !_testItem.FunctionName.IsEmpty();
            result &= !(_testItem.Force == null);
            result &= !_testItem.LimitsId.IsEmpty();

            return result;
        }

        private void ExecuteAddCommand()
        {
            _parameterList.Add(new AdditionalParameters());
        }

        private void ExecuteDeleteCommand(AdditionalParameters parameters)
        {
            _parameterList.Remove(parameters);
        }

        private async Task ExecuteOKCommand()
        {
            try
            {
                var additionalParameters = _parameterList.Where(x => !x.Parameter.IsEmpty()).Select(x => x.Parameter);
                _testItem.AdditionInfo = string.Join(",", additionalParameters);

                if (_testItem.Id.IsEmpty())
                    await _testItemInfoBLL?.CreateAsync(_testItem);
                else
                    await _testItemInfoBLL?.UpdateAsync(_testItem);

                RaiseRequestClose(new DialogResult(ButtonResult.OK));
            }
            catch (Exception e)
            {
                _eventAggregator.GetEvent<ShowShellToastEvent>().Publish(new Toast()
                {
                    Content = e.Message,
                    Type = UI.WPF.Enums.NotificationType.Error,
                });
            }

        }

        private void ExecuteCancelCommand()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public async void OnDialogOpened(IDialogParameters parameters)
        {
            var id = parameters.GetValue<string>("TestItemId");
            var projectInfo = _projectBLL?.GetCurrentProjectInfo();
            _projectInfo = projectInfo;

            #region 查找引脚或组信息
            var ovewview = await _pinOverviewBLL?.GetPinOverviewFromProjectIdAsync(projectInfo.Id);
            var groups = await _groupInfoBLL?.GetListByOverviewIdAsync(ovewview?.Id);
            var pins = await _pinInfoBLL?.GetPinInfosFromOvewviewIdAsync(ovewview?.Id);

            foreach (var group in groups)
            {
                var guid = group.Id.ToGuid();
                if (!GroupOrPinDic.ContainsKey(guid))
                {
                    GroupOrPinDic.Add(guid, $"{L["GroupName"]}-{group.GroupName}");
                }
            }

            foreach (var pin in pins)
            {
                var guid = pin.Id.ToGuid();
                if (!GroupOrPinDic.ContainsKey(guid))
                {
                    GroupOrPinDic.Add(guid, $"{L["PinName"]}-{pin.PinName}");
                }
            }

            #endregion

            if (id == null)
            {
                Title = L["AddTestItem"];
                TestItem = new TestItemInfoModel()
                {
                    ProjectInfoId = projectInfo?.Id.ToGuid(),
                };
            }
            else
            {
                Title = L["EditTestItem"];
                TestItem = await _testItemInfoBLL.GetByIdAsync(id);
            }

            if (_testItem != null)
            {
                _testItem.PropertyChanged += TestItemPropertyChanged;
                if (!_testItem.AdditionInfo.IsEmpty())
                {
                    var additions = _testItem.AdditionInfo.Split(',');
                    foreach (var addition in additions)
                        _parameterList.Add(new AdditionalParameters() { Parameter = addition });
                }
            }

            #region 初始化门限字典
            var limits = await _limitsBLL?.GetListByProjectIdAsync(_projectInfo?.Id);
            if (!limits.IsEmpty())
            {
                foreach (var limit in limits)
                {
                    if (!LimitsDic.ContainsKey(limit.Id.ToGuid()))
                    {
                        LimitsDic.Add(limit.Id.ToGuid(), limit.LimitName);
                    }
                }
            }
            #endregion

            #region 初始化电平组字典
            var levelGroups = await _levelGroupBLL?.GetListByProjectIdAsync(projectInfo.Id);
            if (!levelGroups.IsEmpty())
            {
                foreach (var levelGroup in levelGroups)
                {
                    if (!LevelGroupDic.ContainsKey(levelGroup.Id.ToGuid()))
                    {
                        LevelGroupDic.Add(levelGroup.Id.ToGuid(), levelGroup.LevelGroupName);
                    }
                }
            }
            #endregion

            #region 初始化时钟组字典
            var timingGroups = await _timingGroupBLL.GetListByProjectIdAsync(projectInfo.Id);
            if (!timingGroups.IsEmpty())
            {
                foreach (var timingGroup in timingGroups)
                {
                    if (!TimingGroupDic.ContainsKey(timingGroup.Id.ToGuid()))
                    {
                        TimingGroupDic.Add(timingGroup.Id.ToGuid(), timingGroup.TimingGroupName);
                    }
                }
            }
            #endregion
        }

        private void TestItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OKCommand.RaiseCanExecuteChanged();
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose.Invoke(dialogResult);
        }
    }
}
