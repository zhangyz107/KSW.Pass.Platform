using KSW.ATE01.Application.BLLs.Abstractions.Projects;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.BLLs.Implements.TestPlans;
using KSW.ATE01.Application.Events.Projects;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Start.Views.Dialogs.TestPlans;
using KSW.Helpers;
using KSW.Ui;
using NPOI.OpenXmlFormats.Dml.Chart;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Start.ViewModels.TestPlans
{
    public class LevelSettingViewModel : ViewModelBase, INavigationAware
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private readonly IProjectBLL _projectBLL;
        private readonly ILevelGroupBLL _levelGroupBLL;
        private readonly ILevelBLL _levelBLL;
        private string _title;
        private LevelGroupModel _selectLevelGroup;
        private LevelModel _selectLevel;
        private ProjectInfoModel _projectInfo;
        private ObservableCollection<LevelGroupModel> _levelGroupList = new ObservableCollection<LevelGroupModel>();
        private ObservableCollection<LevelModel> _levelList = new ObservableCollection<LevelModel>();
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
        /// 电平组列表
        /// </summary>
        public ObservableCollection<LevelGroupModel> LevelGroupList
        {
            get => _levelGroupList;
            set => SetProperty(ref _levelGroupList, value);
        }

        /// <summary>
        /// 选中的电平组
        /// </summary>
        public LevelGroupModel SelectLevelGroup
        {
            get => _selectLevelGroup;
            set
            {
                if (SetProperty(ref _selectLevelGroup, value))
                {
                    AddLevelCommand.RaiseCanExecuteChanged();
                    ReloadLevelList();
                }
            }
        }

        /// <summary>
        /// 电平列表
        /// </summary>
        public ObservableCollection<LevelModel> LevelList
        {
            get => _levelList;
            set => SetProperty(ref _levelList, value);
        }

        /// <summary>
        /// 选中的电平
        /// </summary>
        public LevelModel SelectLevel
        {
            get => _selectLevel;
            set => SetProperty(ref _selectLevel, value);
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
        /// 添加电平组
        /// </summary>
        private AsyncDelegateCommand _addLevelGroupCommand;
        public AsyncDelegateCommand AddLevelGroupCommand =>
            _addLevelGroupCommand ?? (_addLevelGroupCommand = new AsyncDelegateCommand(ExecuteAddLevelGroupCommand, () => _projectInfo != null));

        /// <summary>
        /// 确认电平组名
        /// </summary>
        private AsyncDelegateCommand<LevelGroupModel> _sureLevelGroupNameCommand;
        public AsyncDelegateCommand<LevelGroupModel> SureLevelGroupNameCommand =>
            _sureLevelGroupNameCommand ?? (_sureLevelGroupNameCommand = new AsyncDelegateCommand<LevelGroupModel>(ExecuteSureLevelGroupNameCommand));

        /// <summary>
        /// 删除电平组名
        /// </summary>
        private AsyncDelegateCommand<LevelGroupModel> _removeLevelGroupNameCommand;
        public AsyncDelegateCommand<LevelGroupModel> RemoveLevelGroupNameCommand =>
            _removeLevelGroupNameCommand ?? (_removeLevelGroupNameCommand = new AsyncDelegateCommand<LevelGroupModel>(ExecuteRemoveLevelGroupNameCommand));

        /// <summary>
        /// 添加电平
        /// </summary>
        private AsyncDelegateCommand _addLevelCommand;
        public AsyncDelegateCommand AddLevelCommand =>
            _addLevelCommand ?? (_addLevelCommand = new AsyncDelegateCommand(ExecuteAddLevelCommand, () => _selectLevelGroup != null && _selectLevelGroup?.Id != null));

        /// <summary>
        /// 编辑电平
        /// </summary>
        private AsyncDelegateCommand<LevelModel> _editLevelCommand;
        public AsyncDelegateCommand<LevelModel> EditLevelCommand =>
            _editLevelCommand ?? (_editLevelCommand = new AsyncDelegateCommand<LevelModel>(ExecuteEditLevelCommand));

        /// <summary>
        /// 删除电平
        /// </summary>
        private AsyncDelegateCommand<LevelModel> _removeLevelCommand;
        public AsyncDelegateCommand<LevelModel> RemoveLevelCommand =>
            _removeLevelCommand ?? (_removeLevelCommand = new AsyncDelegateCommand<LevelModel>(ExecuteRemoveLevelCommand));

        #endregion

        public LevelSettingViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator,
            IProjectBLL projectBLL,
            ILevelGroupBLL levelGroupBLL,
            ILevelBLL levelsBLL) : base(containerProvider)
        {
            Title = L["LevelSetting"];
            L.PropertyChanged += (sender, args) =>
            {
                Title = L["LevelSetting"];
            };
            _eventAggregator = eventAggregator;
            _projectBLL = projectBLL;
            _levelGroupBLL = levelGroupBLL;
            _levelBLL = levelsBLL;

            InitEvent();
        }

        private void InitEvent()
        {
            _eventAggregator.GetEvent<SelectedProjectInfoEvent>().Subscribe(SelectedProjectInfo);
        }

        private async void SelectedProjectInfo()
        {
            var projectInfo = _projectBLL?.GetCurrentProjectInfo();
            if (projectInfo != null)
                ProjectInfo = projectInfo;


            var levelGroupList = await _levelGroupBLL?.GetListByProjectIdAsync(projectInfo.Id);
            if (!levelGroupList.IsEmpty())
            {
                LevelGroupList.Clear();
                LevelGroupList.AddRange(levelGroupList);
            }
            AddLevelGroupCommand.RaiseCanExecuteChanged();
        }

        private async Task ExecuteAddLevelGroupCommand()
        {
            var levelGroup = new LevelGroupModel
            {
                ProjectInfoId = _projectInfo?.Id.ToGuid(),
                IsNew = true,
            };
            _levelGroupList.Add(levelGroup);
        }

        private async Task ExecuteSureLevelGroupNameCommand(LevelGroupModel model)
        {
            if (model.LevelGroupName.IsEmpty())
            {
                await DialogService.ShowMessageDialog($"{L["LevelGroupName"]}{L["CanNotBeEmpty"]}", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            var list = await _levelGroupBLL?.GetListByProjectIdAsync(_projectInfo.Id);
            if (list.Any(x => x.Id != model.Id && x.LevelGroupName == model.LevelGroupName))
            {
                await DialogService.ShowMessageDialog(string.Format(L["FieldAlreadyExists"], model.LevelGroupName), System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                model.LevelGroupName = string.Empty;
                return;
            }

            var levelGroup = await _levelGroupBLL?.SaveAsync(model);
            UpdateLevelGroupModel(model, levelGroup);
            AddLevelCommand.RaiseCanExecuteChanged();
            await DialogService.ShowMessageDialog(L["OperationSuccessful"], System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private void UpdateLevelGroupModel(LevelGroupModel model, LevelGroupModel newGroup)
        {
            model.Id = newGroup.Id;
            model.LevelGroupName = newGroup.LevelGroupName;
            model.Version = newGroup.Version;
            model.CreationTime = newGroup.CreationTime;
            model.LastModificationTime = newGroup.LastModificationTime;
            model.IsNew = false;
        }

        private async Task ExecuteRemoveLevelGroupNameCommand(LevelGroupModel model)
        {
            var dialogResult = await DialogService.ShowMessageDialog(string.Format(L["DeleteConfirm"], model.LevelGroupName), System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
            if (dialogResult.Result != ButtonResult.Yes)
                return;

            if (!model.IsNew)
                await _levelGroupBLL?.DeleteWithChildrenAsync(model.Id);

            _levelGroupList.Remove(model);
        }

        private async Task ExecuteAddLevelCommand()
        {
            var level = new LevelModel
            {
                LevelGroupId = _selectLevelGroup?.Id.ToGuid(),
                LevelGroupName = _selectLevelGroup.LevelGroupName,
                IsNew = true,
            };

            var parameters = new DialogParameters();
            parameters.Add("LevelModel", level);

            await DialogService.ShowDialogAsync(nameof(AddLevelDialogView), parameters);

            await ReloadLevelList();
        }

        private async Task ExecuteEditLevelCommand(LevelModel model)
        {
            var parameters = new DialogParameters();
            parameters.Add("LevelModel", model);

            await DialogService.ShowDialogAsync(nameof(AddLevelDialogView), parameters);
            await ReloadLevelList();
        }

        private async Task ExecuteRemoveLevelCommand(LevelModel model)
        {
            var dialogResult = await DialogService.ShowMessageDialog(string.Format(L["DeleteConfirm"], model.LevelGroupName), System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
            if (dialogResult.Result != ButtonResult.Yes) 
                return;

            await _levelBLL?.DeleteAsync(model.Id);
            _levelList.Remove(model);
        }

        private async Task ReloadLevelList()
        {
            var list = await _levelBLL?.GetListByGroupIdAsync(_selectLevelGroup?.Id);
            if (!list.IsEmpty())
            {
                var index = 0;
                foreach (var item in list)
                    item.SortId = ++index;
                _levelList.Clear();
                _levelList.AddRange(list);
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
