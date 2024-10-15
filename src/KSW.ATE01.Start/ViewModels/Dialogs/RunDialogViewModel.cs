/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：RunDialogViewModel.cs
// 功能描述：运行窗口视图模型
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

namespace KSW.ATE01.Start.ViewModels.Dialogs
{
    /// <summary>
    /// 运行窗口视图模型
    /// </summary>
    public class RunDialogViewModel : ViewModelBase, IDialogAware
    {
        #region Field
        private readonly IEventAggregator _eventAggregator;
        private readonly IProjectBLL _projectBLL;
        private bool? _isAllItemsSelected = false;
        private ProjectInfoModel _projectInfo;
        private int _loopExecuted;
        private int _failCount;

        #endregion

        #region Properties
        public DialogCloseListener RequestClose { get; }

        public string Title => L["Run"];

        public bool? IsAllItemsSelected
        {
            get => _isAllItemsSelected;
            set => SetProperty(ref _isAllItemsSelected, value);
        }

        /// <summary>
        /// 已执行循环
        /// </summary>
        public int LoopExecuted
        {
            get => _loopExecuted;
            set => SetProperty(ref _loopExecuted, value);
        }

        /// <summary>
        /// 失败数
        /// </summary>
        public int FailCount
        {
            get => _failCount;
            set => SetProperty(ref _failCount, value);
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

        public ProjectInfoModel ProjectInfo
        {
            get => _projectInfo;
        }
        #endregion

        public RunDialogViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
            _projectBLL = ContainerProvider?.Resolve<IProjectBLL>();

            LoadData();
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

        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose.Invoke(dialogResult);
        }

        private void LoadData()
        {
            var currentProjectInfo = _projectBLL?.GetCurrentProjectInfo();
            _projectInfo = currentProjectInfo != null ? DeepCopy.Copy(currentProjectInfo) : null;
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
                _projectInfo.DatalogPath = folderName;
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
