using KSW.Data;
using KSW.Ui;
using KSW.UI.WPF.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.UI.WPF.ViewModels
{
    public class MessageDialogViewModel : BindableBase, IDialogAware
    {
        #region Fields
        private MessageDialogParameters _dialogParamters;
        private string _messageText;
        private string _messageIcon;
        private string _buttonOneText;
        private bool _isButtonTwoVisiblity = false;
        private bool _isButtonThreeVisiblity = false;
        #endregion

        #region Properties
        /// <summary>
        /// 消息文字
        /// </summary>
        public string MessageText
        {
            get => _messageText;
            set => SetProperty(ref _messageText, value);
        }

        /// <summary>
        /// 消息图标
        /// </summary>
        public string MessageIcon
        {
            get => _messageIcon;
            set => SetProperty(ref _messageIcon, value);
        }

        /// <summary>
        /// 按钮一文字
        /// </summary>
        public string ButtonOneText
        {
            get => _buttonOneText;
            set => SetProperty(ref _buttonOneText, value);
        }

        /// <summary>
        /// 按钮二是否显示
        /// </summary>
        public bool IsButtonTwoVisiblity
        {
            get => _isButtonTwoVisiblity;
            set => SetProperty(ref _isButtonTwoVisiblity, value);
        }
        /// <summary>
        /// 按钮三是否显示
        /// </summary>
        public bool IsButtonThreeVisiblity
        {
            get => _isButtonThreeVisiblity;
            set => SetProperty(ref _isButtonThreeVisiblity, value);
        }
        #endregion

        #region Command
        private DelegateCommand _oKCommand;
        public DelegateCommand OKCommand =>
            _oKCommand ?? (_oKCommand = new DelegateCommand(ExecuteOKCommand));

        private DelegateCommand _noCommand;

        public DelegateCommand NoCommand =>
            _noCommand ?? (_noCommand = new DelegateCommand(ExecuteNoCommand));

        private DelegateCommand _cancelCommand;

        public DelegateCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(ExecuteCancelCommand));
        #endregion

        public DialogCloseListener RequestClose { get; }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var messageDialogParameters = parameters.GetValue<MessageDialogParameters>("params");
            _dialogParamters = messageDialogParameters;
            MessageText = _dialogParamters.MessageText;
            var basePath = $"pack://application:,,,/{GetType().Assembly.GetName().Name};component/resources/images";
            var messageIcon = basePath + "/{0}";

            switch (_dialogParamters.MessageButton)
            {
                case System.Windows.MessageBoxButton.OK:
                    ButtonOneText = LanguageManager.Instance["Ok"];
                    IsButtonTwoVisiblity = false;
                    IsButtonThreeVisiblity = false;

                    break;
                case System.Windows.MessageBoxButton.OKCancel:
                    ButtonOneText = LanguageManager.Instance["Ok"];
                    IsButtonTwoVisiblity = false;
                    IsButtonThreeVisiblity = true;
                    break;
                case System.Windows.MessageBoxButton.YesNoCancel:
                    ButtonOneText = LanguageManager.Instance["Yes"];
                    IsButtonTwoVisiblity = true;
                    IsButtonThreeVisiblity = true;
                    break;
                case System.Windows.MessageBoxButton.YesNo:
                    ButtonOneText = LanguageManager.Instance["Yes"];
                    IsButtonTwoVisiblity = true;
                    IsButtonThreeVisiblity = false;
                    break;
                default:
                    break;
            }

            switch (_dialogParamters.MessageIcon)
            {
                case System.Windows.MessageBoxImage.None:
                    break;
                case System.Windows.MessageBoxImage.Error:
                    MessageIcon = string.Format(messageIcon, "error.png");
                    break;
                case System.Windows.MessageBoxImage.Question:
                    MessageIcon = string.Format(messageIcon, "question.png");
                    break;
                case System.Windows.MessageBoxImage.Warning:
                    MessageIcon = string.Format(messageIcon, "warning.png");
                    break;
                case System.Windows.MessageBoxImage.Information:
                    MessageIcon = string.Format(messageIcon, "information.png");
                    break;
                default:
                    break;
            }
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose.Invoke(dialogResult);
        }

        private async void ExecuteOKCommand()
        {
            var buttonResult = ButtonResult.OK;
            switch (_dialogParamters.MessageButton)
            {
                case System.Windows.MessageBoxButton.YesNoCancel:
                case System.Windows.MessageBoxButton.YesNo:
                    buttonResult = ButtonResult.Yes;
                    break;
            }

            RaiseRequestClose(new DialogResult(buttonResult));
        }

        private void ExecuteNoCommand()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.No));
        }


        private void ExecuteCancelCommand()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
        }
    }
}
