using KSW.Helpers;
using KSW.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.Pass.Start.ViewModels
{
    public class ShellViewModel : ViewModel
    {
        private readonly IDialogService _dialogService;

        private DelegateCommand _loadingCommand;
        public DelegateCommand LoadingCommand =>
            _loadingCommand ?? (_loadingCommand = new DelegateCommand(ExecuteLoadingCommand));

        public ShellViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        private void ExecuteLoadingCommand()
        {
            var tempParams = ProcessBarHelper.CreateProcessBarParameters(DoWork, false);
            _dialogService.ShowProcessBarDialog(tempParams);
        }

        private async Task DoWork(Action<double, string> action)
        {
            int length = 100;
            for (int i = 0; i < length; i++)
            {
                if (action != null)
                    action(i + 1, $"当前进度：{i + 1}%");
                await Task.Delay(100);
            }
        }
    }
}
