using KSW.Helpers;
using KSW.ATE01.Start.Views;
using KSW.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

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

        private string _language;

        public string Language
        {
            get => _language;
            set
            {
                if (SetProperty(ref _language, value))
                {
                    ChangeLanguage(value);
                }
            }
        }


        public Dictionary<string, string> LanguageCbItems => new Dictionary<string, string>()
        {
            {"zh-CN","简体中文" },
            {"en-US", "English"}
        };
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
            var currentCulture = CultureInfo.CurrentCulture;
            Language = currentCulture.Name;

            ProjectView = _container.Resolve<ProjectView>();
            HelpView = _container.Resolve<HelpView>();
        }


        private void ChangeLanguage(string value)
        {
            CultureInfo culture = new CultureInfo(value);
            LanguageManager.Instance.ChangeLanguage(culture);
        }
    }
}
