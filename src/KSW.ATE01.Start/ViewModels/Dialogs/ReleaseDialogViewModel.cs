/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：ReleaseDialogViewModel.cs
// 功能描述：发布窗口视图模型
//
// 作者：zhangyingzhong
// 日期：2024/10/10 13:41
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.ATE01.Application.BLLs.Abstractions;
using KSW.ATE01.Application.Events.Projects;
using KSW.ATE01.Application.Models.Projects;
using KSW.Helpers;
using KSW.Ui;
using Microsoft.Win32;
using System.IO;

namespace KSW.ATE01.Start.ViewModels.Dialogs
{
    /// <summary>
    /// 发布窗口视图模型
    /// </summary>
    public class ReleaseDialogViewModel : ViewModelBase, IDialogAware
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private readonly IProjectBLL _projectBLL;
        private ProjectInfoModel _projectInfo;
        #endregion

        #region Properties
        public DialogCloseListener RequestClose { get; }

        public string Title => L["Release"];

        public ProjectInfoModel ProjectInfo
        {
            get => _projectInfo;
            private set => SetProperty(ref _projectInfo, value);
        }
        #endregion

        #region Command;
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
        public ReleaseDialogViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
            _projectBLL = ContainerProvider?.Resolve<IProjectBLL>();
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            if (_projectInfo != null && _projectBLL.SaveProjectInfo(_projectInfo))
            {
                _projectBLL.SetCurrentProjectInfo(_projectInfo);
                _eventAggregator.GetEvent<ProjectInfoUpdateEvent>().Publish();
            }
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            LoadData();
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose.Invoke(dialogResult);
        }

        private void LoadData()
        {
            var currentProjectInfo = _projectBLL?.GetCurrentProjectInfo();
            ProjectInfo = currentProjectInfo != null ? DeepCopy.Copy(currentProjectInfo) : null;
        }

        private void ExecuteOpenFolderCommand()
        {
            var folderDialog = new OpenFolderDialog()
            {
                Title = L["SelectFolder"],
            };

            if (!_projectInfo.ReleasePath.IsEmpty() && Directory.Exists(_projectInfo.ReleasePath))
                folderDialog.InitialDirectory = _projectInfo.ReleasePath;

            if (folderDialog.ShowDialog() == true)
            {
                var folderName = folderDialog.FolderName;
                _projectInfo.ReleasePath = folderName;
            }
        }

        private void ExecuteOKCommand()
        {

            RaiseRequestClose(new DialogResult(ButtonResult.OK));
        }

        private void ExecuteCancelCommand()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
        }
    }
}
