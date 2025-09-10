/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：ProjectViewModel.cs
// 功能描述：项目视图模型
//
// 作者：zhangyingzhong
// 日期：2024/10/09 13:46
// 修改记录(Revision History)
// 修改时间：2025/08/12 16:45
// 修改人：zhangyingzhong
//------------------------------------------------------------*/

using KSW.ATE01.Application.BLLs.Abstractions.Projects;
using KSW.ATE01.Application.Events.Projects;
using KSW.ATE01.Application.Events.TestPlans;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Start.Views;
using KSW.ATE01.Start.Views.Dialogs;
using KSW.ATE01.Start.Views.TestPlans;
using KSW.Helpers;
using KSW.Ui;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;

namespace KSW.ATE01.Start.ViewModels
{
    /// <summary>
    /// 项目视图模型
    /// </summary>
    public class ProjectViewModel : ViewModelBase
    {
        #region Fields
        private readonly IContainerExtension _containerProvider;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly IProjectBLL _projectBLL;
        private ObservableCollection<ProjectInfoModel> _projectList = new ObservableCollection<ProjectInfoModel>();
        private ProjectDetailView _projectDetailView;
        private ProjectInfoModel _selectProjectInfo;
        #endregion

        #region Properties
        public ObservableCollection<ProjectInfoModel> ProjectList
        {
            get => _projectList;
            set => SetProperty(ref _projectList, value);
        }

        public ProjectInfoModel SelectProjectInfo
        {
            get => _selectProjectInfo;
            set
            {
                if (SetProperty(ref _selectProjectInfo, value))
                {
                    _projectBLL?.SetCurrentProjectInfo(value);
                    ImportDataCommand.RaiseCanExecuteChanged();
                    ExportDataCommand.RaiseCanExecuteChanged();
                    _eventAggregator.GetEvent<SelectedProjectInfoEvent>().Publish();
                }
            }
        }

        public ProjectDetailView ProjectDetailView
        {
            get => _projectDetailView;
            set => SetProperty(ref _projectDetailView, value);
        }

        #endregion

        #region Command
        private AsyncDelegateCommand _loadingCommand;
        public AsyncDelegateCommand LoadingCommand =>
            _loadingCommand ?? (_loadingCommand = new AsyncDelegateCommand(ExecuteLoadingCommand));

        private AsyncDelegateCommand _newProjectCommand;
        public AsyncDelegateCommand NewProjectCommand =>
            _newProjectCommand ?? (_newProjectCommand = new AsyncDelegateCommand(ExecuteNewProjectCommand));

        private DelegateCommand _saveAsCommand;
        public DelegateCommand SaveAsCommand =>
            _saveAsCommand ?? (_saveAsCommand = new DelegateCommand(ExecuteSaveAsCommand));

        private AsyncDelegateCommand _importDataCommand;
        public AsyncDelegateCommand ImportDataCommand =>
            _importDataCommand ?? (_importDataCommand = new AsyncDelegateCommand(ExecuteImportDataCommand, () => _selectProjectInfo != null));

        private AsyncDelegateCommand _exportDataCommand;
        public AsyncDelegateCommand ExportDataCommand =>
            _exportDataCommand ?? (_exportDataCommand = new AsyncDelegateCommand(ExecuteExportDataCommand, () => _selectProjectInfo != null));

        private AsyncDelegateCommand<ProjectInfoModel> _editCommand;
        public AsyncDelegateCommand<ProjectInfoModel> EditCommand =>
            _editCommand ?? (_editCommand = new AsyncDelegateCommand<ProjectInfoModel>(ExecuteEditCommand));

        private DelegateCommand<ProjectInfoModel> _delelopCommand;
        public DelegateCommand<ProjectInfoModel> DevelopCommand =>
            _delelopCommand ?? (_delelopCommand = new DelegateCommand<ProjectInfoModel>(ExecuteDevelopCommand));

        private AsyncDelegateCommand<ProjectInfoModel> _runCommand;
        public AsyncDelegateCommand<ProjectInfoModel> RunCommand =>
            _runCommand ?? (_runCommand = new AsyncDelegateCommand<ProjectInfoModel>(ExecuteRunCommand));

        private AsyncDelegateCommand<ProjectInfoModel> _deleteCommand;
        public AsyncDelegateCommand<ProjectInfoModel> DeleteCommand =>
            _deleteCommand ?? (_deleteCommand = new AsyncDelegateCommand<ProjectInfoModel>(ExecuteDeleteCommand));

        private DelegateCommand _releaseCommand;
        public DelegateCommand ReleaseCommand =>
            _releaseCommand ?? (_releaseCommand = new DelegateCommand(ExecuteReleaseCommand));

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public ProjectViewModel(
            IContainerExtension containerProvider,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            IProjectBLL projectBLL) : base(containerProvider)
        {
            _containerProvider = containerProvider;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _projectBLL = projectBLL;

            #region 加载页面
            _projectDetailView = _containerProvider.Resolve<ProjectDetailView>();
            #endregion

            InitEvent();
        }

        private void InitEvent()
        {

            _eventAggregator.GetEvent<RefreshProjectListEvent>().Subscribe(async () => await ReloadList());
        }

        private async Task ExecuteLoadingCommand()
        {
            await ReloadList();

            _regionManager.RequestNavigate(RegionNameManagement.TestPlanContent, nameof(ChannelSettingView));
            _regionManager.RequestNavigate(RegionNameManagement.TestPlanContent, nameof(LimitsSettingView));
            _regionManager.RequestNavigate(RegionNameManagement.TestPlanContent, nameof(LevelSettingView));
            _regionManager.RequestNavigate(RegionNameManagement.TestPlanContent, nameof(TimingSettingView));
            _regionManager.RequestNavigate(RegionNameManagement.TestPlanContent, nameof(TestItemSettingView));
            _regionManager.RequestNavigate(RegionNameManagement.TestPlanContent, nameof(GlobalSettingView));

            // 回到通道设置
            _regionManager.RequestNavigate(RegionNameManagement.TestPlanContent, nameof(ChannelSettingView));
        }

        private async Task ReloadList()
        {
            _projectList.Clear();
            var list = await _projectBLL?.GetListAsync();
            _projectList.AddRange(list);
            foreach (var item in _projectList)
            {
                item.EditCommand = EditCommand;
                item.DevelopCommand = DevelopCommand;
                item.RunCommand = RunCommand;
                item.DeleteCommand = DeleteCommand;
            }
        }

        private async Task ExecuteNewProjectCommand()
        {
            await DialogService.ShowDialogAsync(nameof(NewProjectDialog));

            await ReloadList();
        }

        private void ExecuteSaveAsCommand()
        {
            DialogService.ShowDialog(nameof(SaveAsDialog));
        }

        private async Task ExecuteImportDataCommand()
        {
            var fileDialog = new OpenFileDialog()
            {
                Filter = "testplan files(*.xlsm;*.xlsx)|*.xlsm;*.xlsx"
            };
            if (fileDialog.ShowDialog() == true)
            {
                var filePath = fileDialog.FileName;
                await _projectBLL?.ImportTestPlanAsync(filePath);
                await DialogService.ShowMessageDialog(L["ImportSuccessful"], System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                _eventAggregator.GetEvent<UpdateTestPlanEvent>().Publish();
            }
        }

        private async Task ExecuteExportDataCommand()
        {
            var fileDialog = new SaveFileDialog() 
            {
                Filter = "testplan files(*.xlsx)|*.xlsx"
            };
            if (fileDialog.ShowDialog() == true)
            {
                var filePath = fileDialog.FileName;
                //if (File.Exists(filePath))
                //{
                //    var fileName = Path.GetFileName(filePath);
                //    var result = await DialogService.ShowMessageDialog($"{string.Format(L["FileAlreadyExists"], fileName)},{L["WhetherToReplace"]}?", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
                //    if (result.Result != ButtonResult.Yes)
                //        return;                   
                //}
                await _projectBLL?.ExportTestPlanAsync(filePath);
            }
        }

        private async Task ExecuteEditCommand(ProjectInfoModel model)
        {
            var parameters = new DialogParameters();
            parameters.Add("ProjectId", model.Id);

            await DialogService.ShowDialogAsync(nameof(NewProjectDialog), parameters);

            var projectInfo = await _projectBLL?.GetByIdAsync(model.Id);
            UpdateModel(model, projectInfo);
        }

        private void UpdateModel(ProjectInfoModel model, ProjectInfoModel newModel)
        {
            model.Id = newModel.Id;
            model.ProjectName = newModel.ProjectName;
            model.ProjectPath = newModel.ProjectPath;
            model.ProjectVersion = newModel.ProjectVersion;
            model.SaveRealTimeText = newModel.SaveRealTimeText;
            model.SaveSummary = newModel.SaveSummary;
            model.SaveCsv = newModel.SaveCsv;
            model.SaveStdf = newModel.SaveStdf;
            model.DatalogPath = newModel.DatalogPath;
            model.IsDoAll = newModel.IsDoAll;
            model.IsPrintTime = newModel.IsPrintTime;
            model.LoopCount = newModel.LoopCount;
            model.DelayBetweenLoops = newModel.DelayBetweenLoops;
            model.LoopExecuted = newModel.LoopExecuted;
            model.FailCount = newModel.FailCount;
            model.StopOnFail = newModel.StopOnFail;
            model.ReleasePath = newModel.ReleasePath;
            model.CreationTime = newModel.CreationTime;
            model.LastModificationTime = newModel.LastModificationTime;
        }

        private void ExecuteDevelopCommand(ProjectInfoModel projectInfo)
        {
            try
            {
                _projectBLL.RunProjecctByVS();
            }
            catch (Exception e)
            {
                DialogService.ShowMessageDialog(e.Message);
                Log?.LogError(e, e.Message);
            }
        }
        private async Task ExecuteRunCommand(ProjectInfoModel model)
        {
            var currentProjectInfo = _projectBLL?.GetCurrentProjectInfo();
            if (currentProjectInfo == null)
            {
                await DialogService.ShowMessageDialog("未打开项目!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }
            DialogService.ShowDialog(nameof(RunDialog));
        }

        private async Task ExecuteDeleteCommand(ProjectInfoModel model)
        {
            if ((await DialogService.ShowMessageDialog("是否删除?", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question))?.Result == ButtonResult.Yes)
            {
                await _projectBLL?.DeleteAsync(model?.Id);
                _projectList.Remove(model);
                _projectBLL?.SetCurrentProjectInfo(null);
                _eventAggregator.GetEvent<SelectedProjectInfoEvent>().Publish();
                _eventAggregator.GetEvent<UpdateTestPlanEvent>().Publish();
            }
        }

        private void ExecuteReleaseCommand()
        {
            DialogService.ShowDialog(nameof(ReleaseDialog));
        }
    }
}
