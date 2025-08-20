using KSW.ATE01.Application.BLLs.Abstractions.Projects;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Start.Styles;
using KSW.Ui;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace KSW.ATE01.Start.ViewModels.Dialogs.TestPlans
{
    public class AddLevelDialogViewModel : ViewModelBase, IDialogAware
    {
        #region Fields
        private readonly IProjectBLL _projectBLL;
        private readonly IPinOverviewBLL _pinOverviewBLL;
        private readonly IGroupInfoBLL _groupInfoBLL;
        private readonly IPinInfoBLL _pinInfoBLL;
        private readonly ILevelBLL _levelBLL;
        private LevelModel _level;
        private SolidColorBrush _messageBackground;
        private SnackbarMessageQueue _messageQueue;
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

        public Dictionary<Guid, string> GroupOrPinDic { get; private set; } = new Dictionary<Guid, string>();

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

        public AddLevelDialogViewModel(
            IContainerProvider containerProvider,
            IProjectBLL projectBLL,
            IPinOverviewBLL pinOverviewBLL,
            IGroupInfoBLL groupInfoBLL,
            IPinInfoBLL pinInfoBLL,
            ILevelBLL levelBLL) : base(containerProvider)
        {
            _projectBLL = projectBLL;
            _pinOverviewBLL = pinOverviewBLL;
            _groupInfoBLL = groupInfoBLL;
            _pinInfoBLL = pinInfoBLL;
            _levelBLL = levelBLL;

            _messageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(1));
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
            var message = string.Empty;

            if (_level.Vil > _level.Vih)
                message = string.Format(L["ExceedValueError"], nameof(LevelModel.Vil), nameof(LevelModel.Vih));

            if (_level.Vol > _level.Voh)
                message = string.Format(L["ExceedValueError"], nameof(LevelModel.Vol), nameof(LevelModel.Voh));

            if (_level.Iol > _level.Ioh)
                message = string.Format(L["ExceedValueError"], nameof(LevelModel.Iol), nameof(LevelModel.Ioh));

            if (_level.Vcl > _level.Vch)
                message = string.Format(L["ExceedValueError"], nameof(LevelModel.Vcl), nameof(LevelModel.Vch));

            if (!message.IsEmpty())
            {
                MessageBackground = new SolidColorBrush(SnackbarMessageStyle.ErrorColor);
                MessageQueue.Enqueue(message);
                return;
            }

            if (_level.Id.IsEmpty())
                await _levelBLL?.CreateAsync(_level);
            else
                await _levelBLL?.UpdateAsync(_level);

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
