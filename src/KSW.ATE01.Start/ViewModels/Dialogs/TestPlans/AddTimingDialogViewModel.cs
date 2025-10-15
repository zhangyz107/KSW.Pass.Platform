/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：AddTimingDialogViewModel.cs
// 功能描述：添加时钟弹窗视图模型
//
// 作者：zhangyingzhong
// 日期：2025/08/21 11:19
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.ATE01.Application.BLLs.Abstractions.Projects;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Events;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Domain.TestPlan.Core.Enums;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.Ui;
using KSW.UI.WPF.Controls;
using System.ComponentModel;
using System.Windows.Data;

namespace KSW.ATE01.Start.ViewModels.Dialogs.TestPlans
{
    /// <summary>
    /// 添加时钟弹窗视图模型
    /// </summary>
    public class AddTimingDialogViewModel : ViewModelBase, IDialogAware
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private readonly IProjectBLL _projectBLL;
        private readonly IPinOverviewBLL _pinOverviewBLL;
        private readonly IGroupInfoBLL _groupInfoBLL;
        private readonly IPinInfoBLL _pinInfoBLL;
        private readonly ITimingBLL _timingBLL;
        private TimingModel _timing;
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
        /// 时钟
        /// </summary>
        public TimingModel Timing
        {
            get => _timing;
            set => SetProperty(ref _timing, value);
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
                    FilteredItems.Refresh();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<TimingformatType, string> TimingformatDic { get; private set; } = Helpers.Enum.GetEnumAndDescriptionDictionary<TimingformatType>();

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<StrobeModeType, string> StrobeModeDic { get; private set; } = Helpers.Enum.GetEnumAndDescriptionDictionary<StrobeModeType>();
        #endregion

        #region Commands
        private DelegateCommand _loadingCommand;
        public DelegateCommand LoadingCommand =>
            _loadingCommand ?? (_loadingCommand = new DelegateCommand(ExecuteLoadingCommand));

        /// <summary>
        /// 确定
        /// </summary>
        private AsyncDelegateCommand _oKCommand;
        public AsyncDelegateCommand OKCommand =>
            _oKCommand ?? (_oKCommand = new AsyncDelegateCommand(ExecuteOKCommand, () => _timing.Error.IsEmpty()));

        /// <summary>
        /// 取消
        /// </summary>
        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(ExecuteCancelCommand));
        #endregion

        public AddTimingDialogViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator,
            IProjectBLL projectBLL,
            IPinOverviewBLL pinOverviewBLL,
            IGroupInfoBLL groupInfoBLL,
            IPinInfoBLL pinInfoBLL,
            ITimingBLL timingBLL) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
            _projectBLL = projectBLL;
            _pinOverviewBLL = pinOverviewBLL;
            _groupInfoBLL = groupInfoBLL;
            _pinInfoBLL = pinInfoBLL;
            _timingBLL = timingBLL;
        }

        private void ExecuteLoadingCommand()
        {
            FilteredItems = CollectionViewSource.GetDefaultView(GroupOrPinDic);
            FilteredItems.Filter = FilterItems;
        }

        private bool CheckInputValue()
        {
            var result = true;
            result &= !_timing.GroupOrPinId.IsEmpty();
            result &= !_timing.TimingName.IsEmpty();
            result &= !(_timing.Period == null);
            result &= !(_timing.WaveformFormat == null);
            result &= !(_timing.DriveA == null);
            result &= !(_timing.DriveB == null);
            result &= !(_timing.DriveC == null);
            result &= !(_timing.DriveD == null);

            return result;
        }

        private async Task ExecuteOKCommand()
        {
            try
            {
                if (_timing.Id.IsEmpty())
                    await _timingBLL?.CreateAsync(_timing);
                else
                    await _timingBLL?.UpdateAsync(_timing);

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
            Timing = parameters.GetValue<TimingModel>("TimingModel");
            _timing.PropertyChanged += TimingPropertyChanged;
            Title = Timing.Id == null ? L["AddTiming"] : L["EditTiming"];

            var projectInfo = _projectBLL?.GetCurrentProjectInfo();
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
        }

        private void TimingPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OKCommand.RaiseCanExecuteChanged();
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

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose.Invoke(dialogResult);
        }
    }
}
