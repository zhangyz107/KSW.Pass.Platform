/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：NewProjectDialogViewModel.cs
// 功能描述：创建项目窗口视图模型
//
// 作者：zhangyingzhong
// 日期：2024/10/09 13:41
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.ATE01.Application.BLLs.Abstractions.Projects;
using KSW.ATE01.Application.Events.Projects;
using KSW.ATE01.Application.Managers.Abstractions.TestPlans;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Project.Base.Helpers;
using KSW.Exceptions;
using KSW.Helpers;
using KSW.Ui;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace KSW.ATE01.Start.ViewModels.Dialogs
{
    /// <summary>
    /// 创建项目窗口视图模型
    /// </summary>
    public class NewProjectDialogViewModel : ViewModelBase, IDialogAware
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogService _dialogService;
        private readonly IProjectBLL _projectBLL;
        private readonly ITestPlanManager _testPlanBLL;
        private string _title;
        private ProjectInfoModel _projectInfo;
        private string _executeName;
        private bool _canEditProjectName = true;

        #endregion

        #region Properties
        public DialogCloseListener RequestClose { get; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public ProjectInfoModel ProjectInfo
        {
            get => _projectInfo;
            private set => SetProperty(ref _projectInfo, value);
        }

        public string ExecuteName
        {
            get => _executeName;
            set => SetProperty(ref _executeName, value);
        }

        /// <summary>
        /// 是否可以编辑项目名
        /// </summary>
        public bool CanEditProjectName
        {
            get => _canEditProjectName;
            set => SetProperty(ref _canEditProjectName, value);
        }

        #endregion

        #region Command
        private DelegateCommand _openFolderCommand;
        public DelegateCommand OpenFolderCommand =>
            _openFolderCommand ?? (_openFolderCommand = new DelegateCommand(ExecuteOpenFolderCommand));

        private DelegateCommand _oKCommand;
        public DelegateCommand OKCommand =>
            _oKCommand ?? (_oKCommand = new DelegateCommand(ExecuteOKCommand, CanCreateProject));

        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(ExecuteCancelCommand));
        #endregion

        public NewProjectDialogViewModel(
            IContainerExtension containerProvider,
            IEventAggregator eventAggregator,
            IDialogService dialogService) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
            _dialogService = dialogService;
            _projectBLL = containerProvider?.Resolve<IProjectBLL>();
            _testPlanBLL = containerProvider?.Resolve<ITestPlanManager>();
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
            var projectId = parameters.GetValue<string>("ProjectId");

            if (projectId.IsEmpty())
            {
                Title = L["NewProject"];
                ProjectInfo = new ProjectInfoModel();
                _projectInfo.ProjectVersion = new Version("1.0.0000.1").ToString();
            }
            else
            {
                Title = L["EditProject"];
                ProjectInfo = _projectBLL?.GetCurrentProjectInfo();
                ExecuteName = _projectInfo.ProjectName + _projectInfo.ExecuteExtension;
                CanEditProjectName = false;
            }

            if (_projectInfo != null)
                _projectInfo.PropertyChanged += ProjectInfo_PropertyChanged;
            OKCommand.RaiseCanExecuteChanged();
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose.Invoke(dialogResult);
        }

        private void ExecuteOpenFolderCommand()
        {
            var folderDialog = new OpenFolderDialog()
            {
                Title = L["SelectFolder"],

            };

            if (folderDialog.ShowDialog() == true)
            {
                var folderName = folderDialog.FolderName;
                _projectInfo.ProjectPath = Path.Combine(folderName, _projectInfo.ProjectName);
            }
        }

        private bool CanCreateProject()
        {
            if (_projectInfo == null)
                return false;

            return !_projectInfo.ProjectName.IsEmpty() && !_projectInfo.ProjectPath.IsEmpty();
        }

        private async void ExecuteOKCommand()
        {
            await ExecuteWithExceptionHandling(async () =>
            {
                if (_projectInfo.ProjectName.IsEmpty())
                    throw new Warning(string.Format("{0}{1}", L["ProjectName"], L["CanNotBeEmpty"]));

                if (_projectInfo.ProjectPath.IsEmpty())
                    throw new Warning(string.Format("{0}{1}", L["ProjectPath"], L["CanNotBeEmpty"]));

                var processBarParameters = ProcessBarHelper.CreateProcessBarParameters(async (action) =>
                {
                    if (_projectInfo.Id.IsEmpty())
                    {
                        //创建项目
                        var result = await _projectBLL?.CreateAsync(_projectInfo);

                        if (!result.IsEmpty())
                        {
                            //生成Release文件夹
                            await _projectBLL?.ReleaseSolutionAsync(_projectInfo, false);

                            //拷贝测试计划
                            //result = await _projectBLL?.CopyTestPlanAsync(_projectInfo);

                            _eventAggregator.GetEvent<ProjectInfoUpdateEvent>().Publish();
                        }
                    }
                    else
                    {
                        await _projectBLL?.UpdateAsync(_projectInfo);
                    }
                });

                await ProcessBarHelper.ShowProcessBarDialogAsync(_dialogService, processBarParameters);
                RaiseRequestClose(new DialogResult(ButtonResult.OK));
            }, async (e) => await _dialogService.ShowMessageDialog(e.Message, MessageBoxButton.OK, MessageBoxImage.Warning));
        }

        private void ExecuteCancelCommand()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
        }

        private void ProjectInfo_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OKCommand.RaiseCanExecuteChanged();
            if (!e.PropertyName.IsEmpty() && e.PropertyName.Equals(nameof(ProjectInfoModel.ProjectName)))
            {
                if (!_projectInfo.ProjectName.IsEmpty())
                {
                    ExecuteName = _projectInfo.ProjectName + _projectInfo.ExecuteExtension;
                }
                else
                {
                    ExecuteName = string.Empty;
                }
            }
        }
    }
}
