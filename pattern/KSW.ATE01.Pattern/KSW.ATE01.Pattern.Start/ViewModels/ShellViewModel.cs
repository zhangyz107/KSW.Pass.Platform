using KSW.ATE01.Pattern.Application.BLLs.Abstractions.Patterns;
using KSW.ATE01.Pattern.Application.Events;
using KSW.ATE01.Pattern.Application.Models.Instruments;
using KSW.ATE01.Pattern.Domain.Instruments.Core.Extensions;
using KSW.ATE01.Pattern.Start.Views;
using KSW.Ui;
using MaterialDesignColors;
using MaterialDesignColors.ColorManipulation;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;

namespace KSW.ATE01.Pattern.Start.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        #region Fields
        private readonly PaletteHelper _paletteHelper = new();
        private PatternCompilerView _patternCompilerView;
        private PatternEditorView _patternEditorView;
        private ShellView _view;
        #endregion

        #region Properties
        public PatternCompilerView PatternCompilerView
        {
            get => _patternCompilerView;
            set => SetProperty(ref _patternCompilerView, value);
        }

        public PatternEditorView PatternEditorView
        {
            get => _patternEditorView;
            set => SetProperty(ref _patternEditorView, value);
        }

        public ObservableCollection<RecordMessageModel> RecordMessages { get; set; } = new ObservableCollection<RecordMessageModel>();
        #endregion

        #region Command
        private DelegateCommand<object> _loadingCommand;
        public DelegateCommand<object> LoadingCommand =>
            _loadingCommand ?? (_loadingCommand = new DelegateCommand<object>(ExecuteLoadingCommand));

        private DelegateCommand _openCommand;
        public DelegateCommand OpenCommand =>
            _openCommand ?? (_openCommand = new DelegateCommand(ExecuteOpenCommand));

        private DelegateCommand<object> _openMessageCommand;
        public DelegateCommand<object> OpenMessageCommand =>
            _openMessageCommand ?? (_openMessageCommand = new DelegateCommand<object>(ExecuteOpenMessageCommand));

        private DelegateCommand _clearMessageCommand;
        public DelegateCommand ClearMessageCommand =>
            _clearMessageCommand ?? (_clearMessageCommand = new DelegateCommand(ExecuteClearMessageCommand));

        #endregion

        public ShellViewModel(
            IContainerProvider containerProvider,
            IPatternCompilerBLL patternCompilerBLL,
            IEventAggregator eventAggregator) : base(containerProvider)
        {
            _patternCompilerView = containerProvider.Resolve<PatternCompilerView>();
            _patternEditorView = containerProvider.Resolve<PatternEditorView>();

            Theme theme = _paletteHelper.GetTheme();
            eventAggregator.GetEvent<MessageOpenEvent>().Subscribe(MessageOpen);
            eventAggregator.GetEvent<RecordMessageEvent>().Subscribe(AppendWriteLine, ThreadOption.UIThread);
            eventAggregator.GetEvent<SendMessageEvent>().Subscribe(AppendWriteLine, ThreadOption.UIThread);
        }

        private void MessageOpen()
        {
            if (_view?.drawerHost != null)
                DrawerHost.OpenDrawerCommand.Execute(Dock.Right, _view?.drawerHost);
        }

        private void AppendWriteLine(RecordMessageModel model)
        {
            RecordMessages.Add(model);
        }

        private void AppendWriteLine(SendMessageModel model)
        {
            try
            {
                RecordMessages.Add(new RecordMessageModel()
                {
                    RecordTime = DateTime.Now,
                    RecordMessage = $"发送消息:{model.Message?.ToAppendString()}"
                });
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void ExecuteLoadingCommand(object shellView)
        {
            if (shellView is ShellView view)
                _view = view;
            ChangePrimaryColor(Color.FromRgb(59, 59, 59));
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

        private void ExecuteOpenCommand()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "atp files(*.atp)|*.atp";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
            {
                var file = openFileDialog.FileName;
                var fileName = Path.GetFileNameWithoutExtension(file);
                var patternsDir = Path.GetDirectoryName(file);
                var binFileName = Path.Combine(patternsDir, $"{fileName}.bin");

                var releaseDir = Path.GetDirectoryName(patternsDir);
                var xlsms = Directory.GetFiles(releaseDir, "*.xlsm");
                var testPlanFilePath = string.Empty;
                if (!xlsms.IsEmpty())
                {
                    testPlanFilePath = xlsms.FirstOrDefault();
                }
                var testPlanSheetName = "Channel";

                //_patternCompilerBLL.SetCompilerPath(file, testPlanFilePath, testPlanSheetName, binFileName);

                //_patternCompilerBLL.CompilePattern();
            }
        }

        private void ExecuteOpenMessageCommand(object obj)
        {
            if (obj is DrawerHost drawerHost)
                DrawerHost.OpenDrawerCommand.Execute(Dock.Right, drawerHost);
        }

        private void ExecuteClearMessageCommand()
        {
            RecordMessages.Clear();
        }
    }
}
