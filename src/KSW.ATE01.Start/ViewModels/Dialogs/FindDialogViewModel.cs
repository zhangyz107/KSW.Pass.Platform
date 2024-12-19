using KSW.ATE01.Application.Events.RealTimeTxts;
using KSW.ATE01.Application.Models.RealTimeTxt;
using KSW.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KSW.ATE01.Start.ViewModels.Dialogs
{
    public class FindDialogViewModel : ViewModelBase, IDialogAware
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private string _searchContent;
        private bool _isWholeWordMatch;
        private bool _isLoopSearch;
        #endregion

        #region Properties
        public string Title => L["Find"];

        public string SearchContent
        {
            get => _searchContent;
            set => SetProperty(ref _searchContent, value);
        }

        public bool IsWholeWordMatch
        {
            get => _isWholeWordMatch;
            set => SetProperty(ref _isWholeWordMatch, value);
        }

        public bool IsLoopSearch
        {
            get => _isLoopSearch;
            set => SetProperty(ref _isLoopSearch, value);
        }

        #endregion

        #region Command
        private DelegateCommand<object> _searchCommand;
        public DelegateCommand<object> SearchCommand =>
            _searchCommand ?? (_searchCommand = new DelegateCommand<object>(ExecuteSearchCommand));

        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(ExecuteCancelCommand));

        public DialogCloseListener RequestClose { get; }
        #endregion

        public FindDialogViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
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

        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose.Invoke(dialogResult);
        }

        private void ExecuteSearchCommand(object isUpSearch)
        {
            if(bool.TryParse(isUpSearch.ToString(),out bool searchType))
            {
                _eventAggregator.GetEvent<SearchContentEvent>().Publish(
                new SearchInfoModel()
                {
                    IsWholeWordMatch = _isWholeWordMatch,
                    IsLoopSearch = _isLoopSearch,
                    SearchContent = _searchContent,
                    IsUpSearch = searchType
                });
            }

        }

        private void ExecuteCancelCommand()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
        }
    }
}
