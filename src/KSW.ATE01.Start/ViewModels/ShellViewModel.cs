using KSW.Helpers;
using KSW.ATE01.Start.Views;
using KSW.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Start.ViewModels
{
    public class ShellViewModel : ViewModel
    {
        private readonly IContainerExtension _container;
        private readonly IDialogService _dialogService;
        private ProjectView _projectView;
        private HelpView _helpView;

        #region Properties

        public ProjectView ProjectView
        {
            get => _projectView;
            set => SetProperty(ref _projectView, value);
        }


        public HelpView HelpView
        {
            get => _helpView;
            set => SetProperty(ref _helpView, value);
        }


        #endregion

        private DelegateCommand _loadingCommand;
        public DelegateCommand LoadingCommand =>
            _loadingCommand ?? (_loadingCommand = new DelegateCommand(ExecuteLoadingCommand));

        public ShellViewModel(
            IContainerExtension container,
            IDialogService dialogService)
        {
            _container = container;
            _dialogService = dialogService;
        }

        private void ExecuteLoadingCommand()
        {
            //var tempParams = ProcessBarHelper.CreateProcessBarParameters(DoWork, false);
            //_dialogService.ShowProcessBarDialog(tempParams);

            ProjectView = _container.Resolve<ProjectView>();
            HelpView = _container.Resolve<HelpView>();
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
