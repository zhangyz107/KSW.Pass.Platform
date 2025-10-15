/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：TimingSettingViewModel.cs
// 功能描述：时钟设置视图模型
//
// 作者：zhangyingzhong
// 日期：2025/08/20 18:23
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

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
    /// 测试计划时钟设置视图模型
    /// </summary>
    public class TimingSettingViewModel : ViewModelBase, INavigationAware
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private readonly IProjectBLL _projectBLL;
        private readonly ITimingGroupBLL _timingGroupBLL;
        private readonly ITimingBLL _timingBLL;
        private string _title;
        private ObservableCollection<TimingGroupModel> _timingGroupList = new ObservableCollection<TimingGroupModel>();
        private ObservableCollection<TimingModel> _timingList = new ObservableCollection<TimingModel>();
        private TimingGroupModel _selectTimingGroup;
        private TimingModel _selectTiming;
        private ProjectInfoModel _projectInfo;

        #endregion

        #region Properties
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// 时钟组列表
        /// </summary>
        public ObservableCollection<TimingGroupModel> TimingGroupList
        {
            get => _timingGroupList;
            set => SetProperty(ref _timingGroupList, value);
        }

        /// <summary>
        /// 当前选中的时钟组
        /// </summary>
        public TimingGroupModel SelectTimingGroup
        {
            get => _selectTimingGroup;
            set
            {
                if (SetProperty(ref _selectTimingGroup, value))
                {
                    AddTimingCommand.RaiseCanExecuteChanged();
                    ReloadTimingList();
                }
            }
        }

        /// <summary>
        /// 时钟列表
        /// </summary>
        public ObservableCollection<TimingModel> TimingList
        {
            get => _timingList;
            set => SetProperty(ref _timingList, value);
        }

        /// <summary>
        /// 当前选中的时钟
        /// </summary>
        public TimingModel SelectTiming
        {
            get => _selectTiming;
            set => SetProperty(ref _selectTiming, value);
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
        /// 添加时钟组
        /// </summary>
        private AsyncDelegateCommand _addTimingGroupCommand;
        public AsyncDelegateCommand AddTimingGroupCommand =>
            _addTimingGroupCommand ?? (_addTimingGroupCommand = new AsyncDelegateCommand(ExecuteAddTimingGroupCommand, () => _projectInfo != null && !TimingGroupList.Any(x => x.TimingGroupName.IsEmpty())));

        /// <summary>
        /// 确认时钟组名
        /// </summary>
        private AsyncDelegateCommand<TimingGroupModel> _sureTimingGroupNameCommand;
        public AsyncDelegateCommand<TimingGroupModel> SureTimingGroupNameCommand =>
            _sureTimingGroupNameCommand ?? (_sureTimingGroupNameCommand = new AsyncDelegateCommand<TimingGroupModel>(ExecuteSureTimingGroupNameCommand));

        /// <summary>
        /// 删除时钟组名
        /// </summary>
        private AsyncDelegateCommand<TimingGroupModel> _removeTimingGroupNameCommand;
        public AsyncDelegateCommand<TimingGroupModel> RemoveTimingGroupNameCommand =>
            _removeTimingGroupNameCommand ?? (_removeTimingGroupNameCommand = new AsyncDelegateCommand<TimingGroupModel>(ExecuteRemoveTimingGroupNameCommand));

        /// <summary>
        /// 添加时钟
        /// </summary>
        private AsyncDelegateCommand _addTimingCommand;
        public AsyncDelegateCommand AddTimingCommand =>
            _addTimingCommand ?? (_addTimingCommand = new AsyncDelegateCommand(ExecuteAddTimingCommand, () => _selectTimingGroup != null && _selectTimingGroup?.Id != null));

        /// <summary>
        /// 编辑时钟
        /// </summary>
        private AsyncDelegateCommand<TimingModel> _editTimingCommand;
        public AsyncDelegateCommand<TimingModel> EditTimingCommand =>
            _editTimingCommand ?? (_editTimingCommand = new AsyncDelegateCommand<TimingModel>(ExecuteEditTimingCommand));

        /// <summary>
        /// 删除时钟
        /// </summary>
        private AsyncDelegateCommand<TimingModel> _removeTimingCommand;
        public AsyncDelegateCommand<TimingModel> RemoveTimingCommand =>
            _removeTimingCommand ?? (_removeTimingCommand = new AsyncDelegateCommand<TimingModel>(ExecuteRemoveTimingCommand));

        #endregion

        public TimingSettingViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator,
            IProjectBLL projectBLL,
            ITimingGroupBLL timingGroupBLL,
            ITimingBLL timingBLL) : base(containerProvider)
        {
            Title = L["TimingSetting"];
            L.PropertyChanged += (sender, args) =>
            {
                Title = L["TimingSetting"];
            };
            _eventAggregator = eventAggregator;
            _projectBLL = projectBLL;
            _timingGroupBLL = timingGroupBLL;
            _timingBLL = timingBLL;

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
            AddTimingGroupCommand.RaiseCanExecuteChanged();
        }

        private async Task ReloadList()
        {
            _timingGroupList.Clear();
            var levelGroupList = await _timingGroupBLL?.GetListByProjectIdAsync(_projectInfo?.Id);
            if (!levelGroupList.IsEmpty())
            {
                _timingGroupList.AddRange(levelGroupList);
            }
            AddTimingGroupCommand.RaiseCanExecuteChanged();
        }

        private async Task ExecuteAddTimingGroupCommand()
        {
            var timingGroup = new TimingGroupModel()
            {
                ProjectInfoId = _projectInfo?.Id.ToGuid(),
                IsNew = true
            };
            _timingGroupList.Add(timingGroup);
        }

        private async Task ExecuteSureTimingGroupNameCommand(TimingGroupModel model)
        {
            if (model.TimingGroupName.IsEmpty())
            {
                await DialogService.ShowMessageDialog($"{L["TimingGroupName"]}{L["CanNotBeEmpty"]}", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            var list = await _timingGroupBLL?.GetListByProjectIdAsync(_projectInfo.Id);
            if (list.Any(x => x.Id != model.Id && x.TimingGroupName == model.TimingGroupName))
            {
                await DialogService.ShowMessageDialog(string.Format(L["FieldAlreadyExists"], model.TimingGroupName), System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                model.TimingGroupName = string.Empty;
                return;
            }


            var timingGroup = await _timingGroupBLL?.SaveAsync(model);
            UpdateTimingGroupModel(model, timingGroup);
            AddTimingCommand.RaiseCanExecuteChanged();
            await DialogService.ShowMessageDialog(L["OperationSuccessful"], System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            AddTimingGroupCommand.RaiseCanExecuteChanged();
        }

        private void UpdateTimingGroupModel(TimingGroupModel model, TimingGroupModel newModel)
        {
            model.Id = newModel.Id;
            model.TimingGroupName = newModel.TimingGroupName;
            model.Version = newModel.Version;
            model.CreationTime = newModel.CreationTime;
            model.LastModificationTime = newModel.LastModificationTime;
            model.IsNew = false;
        }

        private async Task ExecuteRemoveTimingGroupNameCommand(TimingGroupModel model)
        {
            var dialogResult = await DialogService.ShowMessageDialog(L["ConfirmTheDelete"], System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);

            if (dialogResult.Result != ButtonResult.Yes)
                return;

            try
            {
                if (!model.IsNew)
                    await _timingGroupBLL?.DeleteWithChildrenAsync(model.Id);

                _timingGroupList.Remove(model);
                AddTimingGroupCommand.RaiseCanExecuteChanged();
            }
            catch (Exception e)
            {
                await DialogService.ShowMessageDialog(e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private async Task ExecuteAddTimingCommand()
        {
            var timing = new TimingModel()
            {
                TimingGroupId = _selectTimingGroup?.Id.ToGuid(),
                TimingGroupName = _selectTimingGroup?.TimingGroupName,
                IsNew = true
            };

            var parameters = new DialogParameters();
            parameters.Add("TimingModel", timing);

            await DialogService.ShowDialogAsync(nameof(AddTimingDialogView), parameters);

            await ReloadTimingList();
        }

        private async Task ExecuteEditTimingCommand(TimingModel model)
        {
            var parameters = new DialogParameters();
            parameters.Add("TimingModel", model);

            await DialogService.ShowDialogAsync(nameof(AddTimingDialogView), parameters);
            await ReloadTimingList();
        }

        private async Task ReloadTimingList()
        {
            var list = await _timingBLL?.GetListByGroupIdAsync(_selectTimingGroup?.Id);
            _timingList.Clear();
            if (!list.IsEmpty())
            {
                var index = 0;
                foreach (var item in list)
                    item.SortId = ++index;
                _timingList.AddRange(list);
            }
        }

        private async Task ExecuteRemoveTimingCommand(TimingModel model)
        {
            var dialogResult = await DialogService.ShowMessageDialog(L["ConfirmTheDelete"], System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
            if (dialogResult.Result != ButtonResult.Yes)
                return;

            await _timingBLL?.DeleteAsync(model.Id);
            _timingList.Remove(model);
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
