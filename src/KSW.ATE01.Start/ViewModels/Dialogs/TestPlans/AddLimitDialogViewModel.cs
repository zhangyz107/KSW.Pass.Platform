
/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：AddLimitDialogViewModel.cs
// 功能描述：添加门限对话框视图模型
//
// 作者：zhangyingzhong
// 日期：2025/08/19 15:38
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.ATE01.Application.BLLs.Abstractions.Projects;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Events;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Domain.TestPlan.Core.Enums;
using KSW.Ui;
using KSW.UI.WPF.Controls;
using System.ComponentModel;

namespace KSW.ATE01.Start.ViewModels.Dialogs.TestPlans
{
    /// <summary>
    /// 添加门限对话框视图模型
    /// </summary>
    public class AddLimitDialogViewModel : ViewModelBase, IDialogAware
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private readonly ILimitsBLL _limitsBLL;
        private readonly IProjectBLL _projectBLL;
        private ProjectInfoModel _projectInfo;
        private LimitsModel _limit;
        #endregion

        #region Properties
        public DialogCloseListener RequestClose { get; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// 门限
        /// </summary>
        public LimitsModel Limit
        {
            get => _limit;
            set => SetProperty(ref _limit, value);
        }

        /// <summary>
        /// 被测物结果类型字典
        /// </summary>
        public Dictionary<DUTResultType, string> DutResultDic { get; } = Helpers.Enum.GetEnumAndDescriptionDictionary<DUTResultType>();
        #endregion

        #region Commands
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

        public AddLimitDialogViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator,
            ILimitsBLL limitsBLL,
            IProjectBLL projectBLL) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
            _limitsBLL = limitsBLL;
            _projectBLL = projectBLL;
        }

        private bool CheckInputValue()
        {
            var result = true;

            result &= !_limit.LimitName.IsEmpty();
            result &= !(_limit.TestNumber == null);
            result &= !(_limit.LowLimit == null);
            result &= !(_limit.HighLimit == null);
            result &= !_limit.Units.IsEmpty();
            result &= !(_limit.FailSoftwareBin == null);
            result &= !(_limit.FailHardwareBin == null);
            result &= !(_limit.DutResult == null);

            return result;
        }

        private async Task ExecuteOKCommand()
        {
            try
            {
                if (_limit.Id.IsEmpty())
                    await _limitsBLL?.CreateAsync(_limit);
                else
                    await _limitsBLL?.UpdateAsync(_limit);

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
            var id = parameters.GetValue<string>("LimitId");
            var projectInfo = _projectBLL?.GetCurrentProjectInfo();
            _projectInfo = projectInfo;

            if (id == null)
            {
                Title = L["AddLimit"];
                Limit = new LimitsModel()
                {
                    ProjectInfoId = projectInfo?.Id.ToGuid(),
                };
            }
            else
            {
                Title = L["EditLimit"];
                Limit = await _limitsBLL.GetByIdAsync(id);
            }
            if (_limit != null)
                _limit.PropertyChanged += LimitPropertyChanged;
        }

        private void LimitPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OKCommand.RaiseCanExecuteChanged();
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose.Invoke(dialogResult);
        }
    }
}
