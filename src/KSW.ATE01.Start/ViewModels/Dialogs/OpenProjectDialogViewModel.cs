using KSW.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KSW.ATE01.Start.ViewModels.Dialogs
{
    public class OpenProjectDialogViewModel : ViewModel, IDialogAware
    {
        #region Fields
        private string _folderPath;

        #endregion

        #region Properties
        public DialogCloseListener RequestClose { get; }

        public string Title => LanguageManager.Instance["OpenProject"];

        public string FolderPath
        {
            get => _folderPath;
            set => SetProperty(ref _folderPath, value);
        }

        #endregion

        #region Command
        private DelegateCommand _openFolderCommand;
        public DelegateCommand OpenFolderCommand =>
            _openFolderCommand ?? (_openFolderCommand = new DelegateCommand(ExecuteOpenFolderCommand));

        private DelegateCommand _oKCommand;
        public DelegateCommand OKCommand =>
            _oKCommand ?? (_oKCommand = new DelegateCommand(ExecuteOKCommand));

        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(ExecuteCancelCommand));
        #endregion

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose.Invoke(dialogResult);
        }

        private void ExecuteOpenFolderCommand()
        {
            MessageBox.Show("打开文件夹！");
        }


        private void ExecuteOKCommand()
        {

            RaiseRequestClose(new DialogResult(ButtonResult.OK));
        }

        private void ExecuteCancelCommand()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
        }
    }
}
