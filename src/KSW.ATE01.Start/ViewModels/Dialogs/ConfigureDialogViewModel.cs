using KSW.ATE01.Application.BLLs.Abstractions.RealTimeTxt;
using KSW.ATE01.Application.Models.RealTimeTxt;
using KSW.Ui;
using NPOI.SS.Formula.Functions;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Media;

namespace KSW.ATE01.Start.ViewModels.Dialogs
{
    public class ConfigureDialogViewModel : ViewModelBase, IDialogAware
    {
        #region Fields
        private readonly IConfigureFileBLL _configureFileBLL;
        private ConfigureFileModel _configureFileModel;
        private bool _isDrawerOpen;
        private KeywordModel _selectedKeyword;
        private ObservableCollection<KeywordModel> _keywordList = new ObservableCollection<KeywordModel>();

        #endregion

        #region Properties
        public DialogCloseListener RequestClose { get; }

        public string Title => L["Configure"];

        public List<string> EncodingDic => new List<string>()
        {
            {"UTF8"},
            {"ASCII"},
            {"Unicode"},
        };

        public bool IsDrawerOpen
        {
            get => _isDrawerOpen;
            set => SetProperty(ref _isDrawerOpen, value);
        }

        public ConfigureFileModel ConfigureFileModel
        {
            get => _configureFileModel;
            set => SetProperty(ref _configureFileModel, value);
        }

        public ObservableCollection<KeywordModel> KeywordList => _keywordList;

        public KeywordModel SelectedKeyword
        {
            get => _selectedKeyword;
            set => SetProperty(ref _selectedKeyword, value);
        }

        #endregion

        #region Commands
        private DelegateCommand _addCommand;
        public DelegateCommand AddCommand =>
            _addCommand ?? (_addCommand = new DelegateCommand(ExecuteAddCommand));

        private DelegateCommand<object> _setForegroundCommand;
        public DelegateCommand<object> SetForegroundCommand =>
            _setForegroundCommand ?? (_setForegroundCommand = new DelegateCommand<object>(ExecuteSetForegroundCommand));

        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand =>
            _saveCommand ?? (_saveCommand = new DelegateCommand(ExecuteSaveCommand));

        private DelegateCommand _oKCommand;
        public DelegateCommand OKCommand =>
            _oKCommand ?? (_oKCommand = new DelegateCommand(ExecuteOKCommand));

        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(ExecuteCancelCommand));
        #endregion

        public ConfigureDialogViewModel(
            IContainerProvider containerProvider,
            IConfigureFileBLL configureFileBLL) : base(containerProvider)
        {
            _configureFileBLL = configureFileBLL;
        }

        private void KeywordList_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (sender is IList<KeywordModel> KeywordList)
            {
                var sortId = 0;
                foreach (var keyword in KeywordList)
                {
                    keyword.SortId = ++sortId;
                }
            }
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            ConfigureFileModel = _configureFileBLL.GetConfigureFile();
            KeywordList.CollectionChanged += KeywordList_CollectionChanged;
            InitData();
        }

        private void InitData()
        {
            foreach (var keyword in _configureFileModel.Keywords)
                KeywordList.Add(keyword);
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose.Invoke(dialogResult);
        }

        private void ExecuteAddCommand()
        {
            var newKeyword = new KeywordModel()
            {
                Id = Guid.NewGuid().ToString(),
                SortId = KeywordList.Count + 1,
                Foreground = Colors.Black,
                IsHighlight = true,
                Keyword = string.Empty,
                CreateTime = DateTime.Now,
            };
            KeywordList.Add(newKeyword);
        }

        private void ExecuteSetForegroundCommand(object obj)
        {
            if (obj is KeywordModel keyword)
            {
                IsDrawerOpen = true;

                SelectedKeyword = keyword;
            }
        }

        private void ExecuteSaveCommand()
        {
            _configureFileModel.Keywords = KeywordList.ToList();
            _configureFileBLL.SaveConfigureFileToDisk(_configureFileModel);
        }

        private void ExecuteOKCommand()
        {
            _configureFileModel.Keywords = KeywordList.ToList();
            _configureFileBLL.SaveConfigureFile(_configureFileModel);
            RaiseRequestClose(new DialogResult(ButtonResult.OK));
        }

        private void ExecuteCancelCommand()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
        }
    }
}
