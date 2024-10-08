using KSW.Data;

namespace KSW.UI.WPF.ViewModels
{
    public class ProcessBarDialogViewModel : BindableBase, IDialogAware
    {
        #region Field
        private bool _isIndeterminate = true;
        private string _processContent = "任务进行中...";
        private double _processRate = 0;
        #endregion

        #region Properties
        public bool IsIndeterminate
        {
            get => _isIndeterminate;
            private set => SetProperty(ref _isIndeterminate, value);
        }


        public string ProcessContent
        {
            get => _processContent;
            private set => SetProperty(ref _processContent, value);
        }


        public double ProcessRate
        {
            get => _processRate;
            private set => SetProperty(ref _processRate, value);
        }


        #endregion

        public DialogCloseListener RequestClose { get; }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose.Invoke(dialogResult);
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
            var processBarParameters = parameters.GetValue<ProcessBarParameters>("params");
            IsIndeterminate = processBarParameters.IsIndeterminate;
            ProcessContent = processBarParameters.ProcessContent;
            ProcessRate = processBarParameters.ProcessRate;

            processBarParameters.UpdateProgress = (processRate, processContent) =>
            {
                if (!IsIndeterminate)
                {
                    ProcessRate = processRate;
                    if (!string.IsNullOrEmpty(processContent))
                        ProcessContent = processContent;
                }
            };
            await processBarParameters.DoWork();
            RaiseRequestClose(new DialogResult(ButtonResult.Yes));
        }
    }
}
