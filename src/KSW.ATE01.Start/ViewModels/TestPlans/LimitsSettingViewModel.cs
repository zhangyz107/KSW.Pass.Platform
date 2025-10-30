using KSW.ATE01.Application.BLLs.Abstractions.Projects;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Events.Projects;
using KSW.ATE01.Application.Events.TestPlans;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Start.Views.Dialogs.TestPlans;
using KSW.Helpers;
using KSW.Ui;
using System.Collections.ObjectModel;
using System.Windows;

namespace KSW.ATE01.Start.ViewModels.TestPlans
{
    /// <summary>
    /// 测试项门限设置视图模型
    /// </summary>
    public class LimitsSettingViewModel : ViewModelBase, INavigationAware
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private readonly IProjectBLL _projectBLL;
        private readonly ILimitsBLL _limitsBLL;
        private string _title;
        private ProjectInfoModel _projectInfo;
        private LimitsModel _selectLimit;
        private ObservableCollection<LimitsModel> _limitList = new ObservableCollection<LimitsModel>();
        #endregion

        #region Properties
        public ObservableCollection<LimitsModel> LimitList
        {
            get => _limitList;
            set => SetProperty(ref _limitList, value);
        }

        /// <summary>
        /// 选中门限
        /// </summary>
        public LimitsModel SelectLimit
        {
            get => _selectLimit;
            set
            {
                if (SetProperty(ref _selectLimit, value))
                {
                    RemoveLimitCommand.RaiseCanExecuteChanged();
                }
            }

        }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// 项目信息
        /// </summary>
        public ProjectInfoModel ProjectInfo
        {
            get => _projectInfo;
            set => SetProperty(ref _projectInfo, value);
        }
        #endregion

        #region Commands
        /// <summary>
        /// 加载命令
        /// </summary>
        private AsyncDelegateCommand _loadingCommand;
        public AsyncDelegateCommand LoadingCommand =>
            _loadingCommand ?? (_loadingCommand = new AsyncDelegateCommand(ExecuteLoadingCommand));
        /// <summary>
        /// 添加门限命令
        /// </summary>
        private DelegateCommand _addLimitCommand;
        public DelegateCommand AddLimitCommand =>
            _addLimitCommand ?? (_addLimitCommand = new DelegateCommand(ExecuteAddLimitCommand, () => _projectInfo != null));

        /// <summary>
        /// 编辑门限命令
        /// </summary>
        private DelegateCommand<LimitsModel> _editLimitCommand;
        public DelegateCommand<LimitsModel> EditLimitCommand =>
            _editLimitCommand ?? (_editLimitCommand = new DelegateCommand<LimitsModel>(ExecuteEditLimitCommand));

        /// <summary>
        /// 删除门限命令
        /// </summary>
        private AsyncDelegateCommand _removeLimitCommand;
        public AsyncDelegateCommand RemoveLimitCommand =>
            _removeLimitCommand ?? (_removeLimitCommand = new AsyncDelegateCommand(ExecuteRemoveLimitCommand, () => { return _selectLimit != null; }));

        /// <summary>
        /// 删除门限命令
        /// </summary>
        private AsyncDelegateCommand<LimitsModel> _deleteLimitCommand;
        public AsyncDelegateCommand<LimitsModel> DeleteLimitCommand =>
            _deleteLimitCommand ?? (_deleteLimitCommand = new AsyncDelegateCommand<LimitsModel>(ExecuteDeleteLimitCommand));

        #endregion

        public LimitsSettingViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator,
             IProjectBLL projectBLL,
             ILimitsBLL limitsBLL) : base(containerProvider)
        {
            Title = L["LimitSetting"];
            L.PropertyChanged += (sender, args) =>
            {
                Title = L["LimitSetting"];
            };
            _eventAggregator = eventAggregator;
            _projectBLL = projectBLL;
            _limitsBLL = limitsBLL;

            InitEvent();
        }

        private void InitEvent()
        {
            _eventAggregator.GetEvent<SelectedProjectInfoEvent>().Subscribe(SelectedProjectInfo);
            _eventAggregator.GetEvent<UpdateTestPlanEvent>().Subscribe(SelectedProjectInfo);
        }

        private async Task ExecuteLoadingCommand()
        {
            await ReloadList();
        }

        private async void SelectedProjectInfo()
        {
            var projectInfo = _projectBLL?.GetCurrentProjectInfo();
            if (projectInfo != null)
                ProjectInfo = projectInfo;

            await ReloadList();
            AddLimitCommand.RaiseCanExecuteChanged();
        }

        private async Task ReloadList()
        {
            _limitList.Clear();
            var limitList = await _limitsBLL?.GetListByProjectIdAsync(_projectInfo?.Id);
            if (!limitList.IsEmpty())
            {
                var index = 0;
                foreach (var item in limitList)
                    item.SortId = ++index;
                _limitList.AddRange(limitList);
            }
        }

        private async void ExecuteAddLimitCommand()
        {
            if ((await DialogService.ShowDialogAsync(nameof(AddLimitDialogView)))?.Result == ButtonResult.OK)
                await ReloadList();
        }

        private async void ExecuteEditLimitCommand(LimitsModel model)
        {
            var parameters = new DialogParameters();
            parameters.Add("LimitId", model.Id);

            await DialogService.ShowDialogAsync(nameof(AddLimitDialogView), parameters);
            await ReloadList();
        }


        private async Task ExecuteRemoveLimitCommand()
        {
            var dialogResult = await DialogService.ShowMessageDialog(L["ConfirmTheDelete"], MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (dialogResult?.Result == ButtonResult.Yes)
            {
                try
                {
                    await _limitsBLL?.DeleteAsync(_selectLimit.Id);
                    _limitList.Remove(_selectLimit);
                    RemoveLimitCommand.RaiseCanExecuteChanged();
                }
                catch (Exception e)
                {
                    await DialogService.ShowMessageDialog(e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task ExecuteDeleteLimitCommand(LimitsModel model)
        {
            var dialogResult = await DialogService.ShowMessageDialog(L["ConfirmTheDelete"], MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (dialogResult?.Result == ButtonResult.Yes)
            {
                try
                {
                    await _limitsBLL?.DeleteAsync(model.Id);
                    _limitList.Remove(_selectLimit);
                    RemoveLimitCommand.RaiseCanExecuteChanged();
                }
                catch (Exception e)
                {
                    await DialogService.ShowMessageDialog(e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }
    }
}
