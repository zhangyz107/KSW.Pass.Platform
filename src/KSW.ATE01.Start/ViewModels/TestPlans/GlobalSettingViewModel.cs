/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：GlobalSettingViewModel.cs
// 功能描述：全局设置视图模型
//
// 作者：zhangyingzhong
// 日期：2025/08/25 09:46
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.ATE01.Application.BLLs.Abstractions.Projects;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Events.Projects;
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
    /// 全局设置视图模型
    /// </summary>
    public class GlobalSettingViewModel : ViewModelBase, INavigationAware
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private readonly IProjectBLL _projectBLL;
        private readonly IGlobalParameterBLL _globalParameterBLL;

        private string _title;
        private ProjectInfoModel _projectInfo;
        private GlobalParameterModel _selectGlobalParameter;
        private ObservableCollection<GlobalParameterModel> _globalParameterList = new ObservableCollection<GlobalParameterModel>();
        #endregion

        #region Properties
        /// <summary>
        /// 选中的全局参数
        /// </summary>
        public GlobalParameterModel SelectGlobalParameter
        {
            get => _selectGlobalParameter;
            set => SetProperty(ref _selectGlobalParameter, value);
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
        /// 全局参数列表
        /// </summary>
        public ObservableCollection<GlobalParameterModel> GlobalParameterList
        {
            get => _globalParameterList;
            set => SetProperty(ref _globalParameterList, value);
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
        /// 添加全局参数命令
        /// </summary>
        private DelegateCommand _addGlobalParameterCommand;
        public DelegateCommand AddGlobalParameterCommand =>
            _addGlobalParameterCommand ?? (_addGlobalParameterCommand = new DelegateCommand(ExecuteAddGlobalParameterCommand, () => _projectInfo != null));

        /// <summary>
        /// 编辑门限命令
        /// </summary>
        private DelegateCommand<GlobalParameterModel> _editGlobalParameterCommand;
        public DelegateCommand<GlobalParameterModel> EditGlobalParameterCommand =>
            _editGlobalParameterCommand ?? (_editGlobalParameterCommand = new DelegateCommand<GlobalParameterModel>(ExecuteEditGlobalParameterCommand));

        /// <summary>
        /// 删除门限命令
        /// </summary>
        private AsyncDelegateCommand _removeGlobalParameterCommand;
        public AsyncDelegateCommand RemoveGlobalParameterCommand =>
            _removeGlobalParameterCommand ?? (_removeGlobalParameterCommand = new AsyncDelegateCommand(ExecuteRemoveGlobalParameterCommand, () => { return _selectGlobalParameter != null; }));

        /// <summary>
        /// 删除门限命令
        /// </summary>
        private AsyncDelegateCommand<GlobalParameterModel> _deleteGlobalParameterCommand;
        public AsyncDelegateCommand<GlobalParameterModel> DeleteGlobalParameterCommand =>
            _deleteGlobalParameterCommand ?? (_deleteGlobalParameterCommand = new AsyncDelegateCommand<GlobalParameterModel>(ExecuteDeleteGlobalParameterCommand));
        #endregion

        public GlobalSettingViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator,
            IProjectBLL projectBLL,
            IGlobalParameterBLL globalParameterBLL) : base(containerProvider)
        {
            Title = L["GlobalParameters"];
            L.PropertyChanged += (sender, args) =>
            {
                Title = L["GlobalParameters"];
            };

            _eventAggregator = eventAggregator;
            _projectBLL = projectBLL;
            _globalParameterBLL = globalParameterBLL;

            InitEvent();
        }

        private void InitEvent()
        {
            _eventAggregator.GetEvent<SelectedProjectInfoEvent>().Subscribe(SelectedProjectInfo);
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
            AddGlobalParameterCommand.RaiseCanExecuteChanged();
        }

        private async Task ReloadList()
        {
            var globalParameterList = await _globalParameterBLL?.GetListByProjectIdAsync(_projectInfo?.Id);
            _globalParameterList.Clear();
            if (!globalParameterList.IsEmpty())
            {
                var index = 0;
                foreach (var item in globalParameterList)
                    item.SortId = ++index;
                _globalParameterList.AddRange(globalParameterList);
            }
        }

        private async void ExecuteAddGlobalParameterCommand()
        {
            await DialogService.ShowDialogAsync(nameof(AddGlobalParametersDialogView));

            await ReloadList();
        }

        private async void ExecuteEditGlobalParameterCommand(GlobalParameterModel model)
        {
            var parameters = new DialogParameters();
            parameters.Add("GlobalParameterId", model.Id);

            await DialogService.ShowDialogAsync(nameof(AddGlobalParametersDialogView), parameters);
            var testItemInfo = await _globalParameterBLL.GetByIdAsync(model.Id);
            UpdateModel(model, testItemInfo);
        }

        private void UpdateModel(GlobalParameterModel model, GlobalParameterModel newModel)
        {
            model.PatternFile = newModel.PatternFile;
            model.AdditionInfo = newModel.AdditionInfo;
            model.LastModificationTime = newModel.LastModificationTime;
            model.Version = newModel.Version;
        }

        private async Task ExecuteRemoveGlobalParameterCommand()
        {
            var dialogResult = await DialogService.ShowMessageDialog(L["ConfirmTheDelete"], MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (dialogResult?.Result == ButtonResult.Yes)
            {
                await _globalParameterBLL?.DeleteAsync(_selectGlobalParameter?.Id);
                _globalParameterList.Remove(_selectGlobalParameter);
                RemoveGlobalParameterCommand.RaiseCanExecuteChanged();
            }
        }

        private async Task ExecuteDeleteGlobalParameterCommand(GlobalParameterModel model)
        {
            var dialogResult = await DialogService.ShowMessageDialog(L["ConfirmTheDelete"], MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (dialogResult?.Result == ButtonResult.Yes)
            {
                await _globalParameterBLL?.DeleteAsync(model?.Id);
                _globalParameterList.Remove(model);
                RemoveGlobalParameterCommand.RaiseCanExecuteChanged();
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
