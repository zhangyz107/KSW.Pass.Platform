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
using System.Threading.Tasks;
using System.Windows;

namespace KSW.ATE01.Start.ViewModels.TestPlans
{
    public class TestItemSettingViewModel : ViewModelBase, INavigationAware
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private readonly ITestItemInfoBLL _testItemInfoBLL;
        private readonly IProjectBLL _projectBLL;
        private string _title;
        private ProjectInfoModel _projectInfo;
        private TestItemInfoModel _selectTestItem;
        private ObservableCollection<TestItemInfoModel> _testItemList = new ObservableCollection<TestItemInfoModel>();

        #endregion

        #region Properties

        /// <summary>
        /// 当前选中的测试项
        /// </summary>
        public TestItemInfoModel SelectTestItem
        {
            get => _selectTestItem;
            set => SetProperty(ref _selectTestItem, value);
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

        /// <summary>
        /// 测试项列表
        /// </summary>
        public ObservableCollection<TestItemInfoModel> TestItemList
        {
            get => _testItemList;
            set => SetProperty(ref _testItemList, value);
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
        /// 添加测试项命令
        /// </summary>
        private DelegateCommand _addTestItemCommand;
        public DelegateCommand AddTestItemCommand =>
            _addTestItemCommand ?? (_addTestItemCommand = new DelegateCommand(ExecuteAddTestItemCommand, () => _projectInfo != null));

        /// <summary>
        /// 编辑门限命令
        /// </summary>
        private DelegateCommand<TestItemInfoModel> _editTestItemCommand;
        public DelegateCommand<TestItemInfoModel> EditTestItemCommand =>
            _editTestItemCommand ?? (_editTestItemCommand = new DelegateCommand<TestItemInfoModel>(ExecuteEditTestItemCommand));

        /// <summary>
        /// 删除门限命令
        /// </summary>
        private AsyncDelegateCommand _removeTestItemCommand;
        public AsyncDelegateCommand RemoveTestItemCommand =>
            _removeTestItemCommand ?? (_removeTestItemCommand = new AsyncDelegateCommand(ExecuteRemoveTestItemCommand, () => { return _selectTestItem != null; }));

        /// <summary>
        /// 删除门限命令
        /// </summary>
        private AsyncDelegateCommand<TestItemInfoModel> _deleteTestItemCommand;
        public AsyncDelegateCommand<TestItemInfoModel> DeleteTestItemCommand =>
            _deleteTestItemCommand ?? (_deleteTestItemCommand = new AsyncDelegateCommand<TestItemInfoModel>(ExecuteDeleteTestItemCommand));

        #endregion

        public TestItemSettingViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator,
            IProjectBLL projectBLL,
            ITestItemInfoBLL testItemInfoBLL) : base(containerProvider)
        {
            Title = L["TestItemSetting"];
            L.PropertyChanged += (sender, args) =>
            {
                Title = L["TestItemSetting"];
            };

            _eventAggregator = eventAggregator;
            _projectBLL = projectBLL;
            _testItemInfoBLL = testItemInfoBLL;

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

            AddTestItemCommand.RaiseCanExecuteChanged();
        }

        private async void ExecuteAddTestItemCommand()
        {
            await DialogService.ShowDialogAsync(nameof(AddTestItemDialogView));

            await ReloadList();
        }

        private async Task ReloadList()
        {
            _testItemList.Clear();
            var testItemList = await _testItemInfoBLL?.GetListByProjectIdAsync(_projectInfo?.Id);
            if (!testItemList.IsEmpty())
            {
                var index = 0;
                foreach (var item in testItemList)
                    item.SortId = ++index;
                _testItemList.AddRange(testItemList);
            }
        }

        private async void ExecuteEditTestItemCommand(TestItemInfoModel model)
        {
            var parameters = new DialogParameters();
            parameters.Add("TestItemId", model.Id);

            await DialogService.ShowDialogAsync(nameof(AddTestItemDialogView), parameters);
            var testItemInfo = await _testItemInfoBLL.GetByIdAsync(model.Id);
            UpdateModel(model, testItemInfo);
        }

        private void UpdateModel(TestItemInfoModel model, TestItemInfoModel newModel)
        {
            model.TestItemName = newModel.TestItemName;
            model.FunctionName = newModel.FunctionName;
            model.Force = newModel.Force;
            model.GroupOrPinId = newModel.GroupOrPinId;
            model.PinOrGroupName = newModel.PinOrGroupName;
            model.LimitsId = newModel.LimitsId;
            model.LimitName = newModel.LimitName;
            model.LevelGroupId = newModel.LevelGroupId;
            model.LevelGroupName = newModel.LevelGroupName;
            model.TimingGroupId = newModel.TimingGroupId;
            model.TimingGroupName = newModel.TimingGroupName;
            model.AdditionInfo = newModel.AdditionInfo;
            model.LastModificationTime = newModel.LastModificationTime;
            model.Version = newModel.Version;
        }

        private async Task ExecuteRemoveTestItemCommand()
        {
            if (await CheckLimitOccupancy(_selectTestItem))
            {
                var dialogResult = await DialogService.ShowMessageDialog(L["ConfirmTheDelete"], MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (dialogResult?.Result == ButtonResult.Yes)
                {
                    await _testItemInfoBLL?.DeleteAsync(_selectTestItem?.Id);
                    _testItemList.Remove(_selectTestItem);
                    RemoveTestItemCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private async Task ExecuteDeleteTestItemCommand(TestItemInfoModel model)
        {
            if (await CheckLimitOccupancy(model))
            {
                var dialogResult = await DialogService.ShowMessageDialog(L["ConfirmTheDelete"], MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (dialogResult?.Result == ButtonResult.Yes)
                {
                    await _testItemInfoBLL?.DeleteAsync(model.Id);
                    _testItemList.Remove(_selectTestItem);
                    RemoveTestItemCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private async Task<bool> CheckLimitOccupancy(TestItemInfoModel model)
        {
            var result = true;

            return result;
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
