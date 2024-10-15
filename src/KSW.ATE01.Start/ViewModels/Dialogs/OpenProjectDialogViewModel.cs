/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：OpenProjectDialogViewModel.cs
// 功能描述：打开项目窗口视图模型
//
// 作者：zhangyingzhong
// 日期：2024/10/09 13:41
// 修改记录(Revision History)
//
//------------------------------------------------------------*/


using KSW.ATE01.Application.BLLs.Abstractions;
using KSW.ATE01.Application.Events.Projects;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Domain.Projects.Entities;
using KSW.Helpers;
using KSW.Ui;
using Microsoft.CodeAnalysis;
using Microsoft.Win32;
using Prism.Ioc;
using System.IO;
using System.Windows;

namespace KSW.ATE01.Start.ViewModels.Dialogs
{
    /// <summary>
    /// 打开项目窗口视图模型
    /// </summary>
    public class OpenProjectDialogViewModel : ViewModelBase, IDialogAware
    {
        #region Fields
        private string _folderPath;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogService _dialogService;
        private readonly IProjectBLL _projectBLL;
        private List<ProjectInfoModel> _projectList = new List<ProjectInfoModel>();
        private ProjectInfoModel _selectProject;

        #endregion

        #region Properties
        public DialogCloseListener RequestClose { get; }

        public string Title => L["OpenProject"];

        public string FolderPath
        {
            get => _folderPath;
            set => SetProperty(ref _folderPath, value);
        }

        public List<ProjectInfoModel> ProjectList
        {
            get => _projectList;
            set => SetProperty(ref _projectList, value);
        }


        public ProjectInfoModel SelectProject
        {
            get => _selectProject;
            set => SetProperty(ref _selectProject, value);
        }

        #endregion

        #region Command
        private DelegateCommand _openFolderCommand;
        public DelegateCommand OpenFolderCommand =>
            _openFolderCommand ?? (_openFolderCommand = new DelegateCommand(ExecuteOpenFolderCommand));

        private DelegateCommand _oKCommand;
        public DelegateCommand OKCommand =>
            _oKCommand ?? (_oKCommand = new DelegateCommand(ExecuteOKCommand));

        private DelegateCommand _cancelCommand;

        public DelegateCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(ExecuteCancelCommand));
        #endregion

        public OpenProjectDialogViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator,
            IDialogService dialogService) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
            _projectBLL = ContainerProvider.Resolve<IProjectBLL>();
            _dialogService = dialogService;
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

        private void ExecuteOpenFolderCommand()
        {
            var folderDialog = new OpenFolderDialog()
            {
                Title = LanguageManager.Instance["SelectFolder"],
            };

            var currentProjectInfo = _projectBLL?.GetCurrentProjectInfo();

            if ((!currentProjectInfo?.ProjectPath.IsEmpty()) == true && Directory.Exists(currentProjectInfo?.ProjectPath))
                folderDialog.InitialDirectory = currentProjectInfo.ProjectPath;

            if (folderDialog.ShowDialog() == true)
            {
                var folderName = folderDialog.FolderName;
                FolderPath = folderName;

                ProjectList = _projectBLL?.ScanProjects(folderName);
            }
        }


        private void ExecuteOKCommand()
        {
            if (_selectProject == null)
            {
                _dialogService.ShowMessageDialog("没有选择任何项目");
                return;
            }

            _projectBLL?.SetCurrentProjectInfo(_selectProject);
            _eventAggregator.GetEvent<ProjectInfoUpdateEvent>().Publish();
            RaiseRequestClose(new DialogResult(ButtonResult.OK));
        }

        private void ExecuteCancelCommand()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
        }
    }
}
