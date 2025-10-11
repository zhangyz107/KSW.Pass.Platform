/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：AddLevelDialogViewModel.cs
// 功能描述：添加电平弹窗视图模型
//
// 作者：zhangyingzhong
// 日期：2025/08/20 11:41
// 修改记录(Revision History)
//
//------------------------------------------------------------*/


using KSW.ATE01.Application.BLLs.Abstractions.Projects;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Events;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.Ui;
using KSW.UI.WPF.Controls;
using System.ComponentModel;
using System.Windows.Data;

namespace KSW.ATE01.Start.ViewModels.Dialogs.TestPlans
{
    /// <summary>
    /// 添加电平弹窗视图模型
    /// </summary>
    public class AddLevelDialogViewModel : ViewModelBase, IDialogAware
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private readonly IProjectBLL _projectBLL;
        private readonly IPinOverviewBLL _pinOverviewBLL;
        private readonly IGroupInfoBLL _groupInfoBLL;
        private readonly IPinInfoBLL _pinInfoBLL;
        private readonly ILevelBLL _levelBLL;
        private LevelModel _level;
        private string _editString;
        private ICollectionView _filteredItems;
        #endregion

        #region Properties
        public DialogCloseListener RequestClose { get; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// 电平
        /// </summary>
        public LevelModel Level
        {
            get => _level;
            set => SetProperty(ref _level, value);
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
            _oKCommand ?? (_oKCommand = new AsyncDelegateCommand(ExecuteOKCommand, CheckInputValue));

        /// <summary>
        /// 取消
        /// </summary>
        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(ExecuteCancelCommand));

        #endregion

        public AddLevelDialogViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator,
            IProjectBLL projectBLL,
            IPinOverviewBLL pinOverviewBLL,
            IGroupInfoBLL groupInfoBLL,
            IPinInfoBLL pinInfoBLL,
            ILevelBLL levelBLL) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
            _projectBLL = projectBLL;
            _pinOverviewBLL = pinOverviewBLL;
            _groupInfoBLL = groupInfoBLL;
            _pinInfoBLL = pinInfoBLL;
            _levelBLL = levelBLL;
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

            result &= !_level.GroupOrPinId.IsEmpty();
            result &= !(_level.Vil == null);
            result &= !(_level.Vih == null);
            result &= !(_level.Vol == null);
            result &= !(_level.Voh == null);
            result &= !(_level.Iol == null);
            result &= !(_level.Ioh == null);
            result &= !(_level.Vt == null);
            result &= !(_level.Vcl == null);
            result &= !(_level.Vch == null);

            return result;
        }

        private async Task ExecuteOKCommand()
        {
            try
            {
                if (_level.Id.IsEmpty())
                    await _levelBLL?.CreateAsync(_level);
                else
                    await _levelBLL?.UpdateAsync(_level);

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
            Level = parameters.GetValue<LevelModel>("LevelModel");
            _level.PropertyChanged += LevelPropertyChanged;
            Title = Level.Id == null ? L["AddLevel"] : L["EditLevel"];

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

        private void LevelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OKCommand.RaiseCanExecuteChanged();
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose.Invoke(dialogResult);
        }
    }
}
