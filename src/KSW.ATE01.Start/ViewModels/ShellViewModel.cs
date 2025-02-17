/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：ShellViewModel.cs
// 功能描述：主窗口视图模型
//
// 作者：zhangyingzhong
// 日期：2024/10/09 13:41
// 修改记录(Revision History)
//
//------------------------------------------------------------*/


using KSW.ATE01.Start.Views;
using KSW.Ui;
using MaterialDesignColors;
using MaterialDesignColors.ColorManipulation;
using MaterialDesignColors.Recommended;
using MaterialDesignThemes.Wpf;
using System.Globalization;
using System.Windows.Media;

namespace KSW.ATE01.Start.ViewModels
{
    /// <summary>
    /// 主窗口视图模型
    /// </summary>
    public class ShellViewModel : ViewModelBase
    {
        private readonly IContainerExtension _containerProvider;
        private readonly IDialogService _dialogService;
        private readonly PaletteHelper _paletteHelper = new();

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

        public string Title { get => "ATE01"; }

        //public LanguageManager L => LanguageManager.Instance;
        #endregion

        #region Command
        private DelegateCommand _loadingCommand;
        public DelegateCommand LoadingCommand =>
            _loadingCommand ?? (_loadingCommand = new DelegateCommand(ExecuteLoadingCommand));
        #endregion

        public ShellViewModel(
            IContainerExtension containerProvider,
            IDialogService dialogService
            ) : base(containerProvider)
        {
            _containerProvider = containerProvider;
            _dialogService = dialogService;

            Theme theme = _paletteHelper.GetTheme();
        }

        private void ExecuteLoadingCommand()
        {
            ChangePrimaryColor(BlueSwatch.Blue300);

            var currentCulture = CultureInfo.CurrentCulture;
            Language = currentCulture.Name;

            ProjectView = _containerProvider.Resolve<ProjectView>();
            HelpView = _containerProvider.Resolve<HelpView>();
        }


        private void ChangeLanguage(string value)
        {
            CultureInfo culture = new CultureInfo(value);
            LanguageManager.Instance.ChangeLanguage(culture);
        }

        private void ChangePrimaryColor(Color color)
        {
            Theme theme = _paletteHelper.GetTheme();

            theme.PrimaryLight = new ColorPair(color.Lighten());
            theme.PrimaryMid = new ColorPair(color);
            theme.PrimaryDark = new ColorPair(color.Darken());
            theme.SetPrimaryColor(color);
            _paletteHelper.SetTheme(theme);
        }
    }
}
