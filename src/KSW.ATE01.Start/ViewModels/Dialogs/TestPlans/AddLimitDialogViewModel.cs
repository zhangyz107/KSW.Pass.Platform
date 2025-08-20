using KSW.ATE01.Application.BLLs.Abstractions.Projects;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Domain.TestPlan.Core.Enums;
using KSW.ATE01.Start.Styles;
using KSW.Ui;
using MaterialDesignThemes.Wpf;
using System.ComponentModel;
using System.Windows.Media;

namespace KSW.ATE01.Start.ViewModels.Dialogs.TestPlans
{
    public class AddLimitDialogViewModel : ViewModelBase, IDialogAware
    {
        #region Fields
        private readonly ILimitsBLL _limitsBLL;
        private readonly IProjectBLL _projectBLL;
        private ProjectInfoModel _projectInfo;
        private LimitsModel _limit;
        private SnackbarMessageQueue _messageQueue;
        private SolidColorBrush _messageBackground;
        #endregion

        #region Properties
        public DialogCloseListener RequestClose { get; }

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
        /// 提示消息
        /// </summary>
        public SnackbarMessageQueue MessageQueue
        {
            get => _messageQueue;
            set => SetProperty(ref _messageQueue, value);
        }

        /// <summary>
        /// 提示消息的背景色
        /// </summary>
        public SolidColorBrush MessageBackground
        {
            get => _messageBackground;
            set => SetProperty(ref _messageBackground, value);
        }

        /// <summary>
        /// 被测物结果类型字典
        /// </summary>
        public Dictionary<DUTResultType, string> DutResultDic { get; } = new Dictionary<DUTResultType, string>()
        {
            {DUTResultType.None, DUTResultType.None.Description()},
            {DUTResultType.Pass, DUTResultType.Pass.Description()},
            {DUTResultType.Fail, DUTResultType.Fail.Description()},
            {DUTResultType.Error, DUTResultType.Error.Description()}
        };
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
            ILimitsBLL limitsBLL,
            IProjectBLL projectBLL) : base(containerProvider)
        {
            _limitsBLL = limitsBLL;
            _projectBLL = projectBLL;

            _messageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(1));
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
            var message = string.Empty;
            if (_limit.LowLimit > _limit.HighLimit)
                message = L["LimitValueError"];

            var limitList = await _limitsBLL?.GetListByProjectIdAsync(_projectInfo?.Id);
            if (!limitList.IsEmpty() && limitList.Any(x => x.LimitName == _limit.LimitName && x.Id != _limit.Id))
                message = string.Format(L["FieldAlreadyExists"], _limit.LimitName);

            if (!message.IsEmpty())
            {
                MessageBackground = new SolidColorBrush(SnackbarMessageStyle.ErrorColor);
                MessageQueue.Enqueue(message);
                return;
            }

            if (_limit.Id.IsEmpty())
                await _limitsBLL?.CreateAsync(_limit);
            else
                await _limitsBLL?.UpdateAsync(_limit);

            RaiseRequestClose(new DialogResult(ButtonResult.OK));
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
