/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：SaveAsDialogViewModel.cs
// 功能描述：另存为窗口视图模型
//
// 作者：zhangyingzhong
// 日期：2024/10/09 13:41
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.ATE01.Application.BLLs.Abstractions.Projects;
using KSW.ATE01.Application.BLLs.Implements.Projects;
using KSW.ATE01.Application.Events.Projects;
using KSW.ATE01.Application.Managers.Abstractions.Projects;
using KSW.ATE01.Application.Managers.Abstractions.TestPlans;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Domain.Projects.Core.Enums;
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
    /// 另存为窗口视图模型
    /// </summary>
    public class SaveAsDialogViewModel : ViewModelBase, IDialogAware
    {
        #region Fields
        private readonly IDialogService _dialogService;
        private readonly IProjectBLL _projectBLL;
        private ProjectInfoModel _currentProjectInfo;
        private string _currentProjectPath;
        private string _version;
        private string _saveAsDir;
        private string _saveAsName;

        #endregion

        #region Properties
        public DialogCloseListener RequestClose { get; }
        public string Title => L["SaveAs"];

        /// <summary>
        /// 当前项目路径
        /// </summary>
        public string CurrentProjectPath
        {
            get => _currentProjectPath;
            set => SetProperty(ref _currentProjectPath, value);
        }

        /// <summary>
        /// 另存为路径
        /// </summary>
        public string SaveAsDir
        {
            get => _saveAsDir;
            set => SetProperty(ref _saveAsDir, value);
        }


        public string SaveAsName
        {
            get => _saveAsName;
            set => SetProperty(ref _saveAsName, value);
        }

        /// <summary>
        /// 版本
        /// </summary>
        public string Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
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

        public SaveAsDialogViewModel(
            IContainerProvider containerProvider,
            IDialogService dialogService,
            IProjectBLL projectBLL) : base(containerProvider)
        {
            _dialogService = dialogService;
            _projectBLL = projectBLL;

            LoadData();
        }

        private void LoadData()
        {
            _currentProjectInfo = _projectBLL?.GetCurrentProjectInfo();
            _version = _currentProjectInfo?.ProjectVersion;
            _currentProjectPath = _currentProjectInfo?.ProjectPath;
            _saveAsDir = Path.GetDirectoryName(_currentProjectInfo?.ProjectPath);
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
                Title = L["SelectFolder"],

            };

            if (folderDialog.ShowDialog() == true)
            {
                var folderName = folderDialog.FolderName;
                SaveAsDir = folderName;
            }
        }

        private async void ExecuteOKCommand()
        {
            await ExecuteWithExceptionHandling(async () =>
            {
                if (_currentProjectInfo == null)
                    throw new Warning(string.Format("{0}{1}", L["SelectProject"], L["IsEmpty"]));

                if (_version.IsEmpty())
                    throw new Warning(string.Format("{0}{1}", L["Version"], L["CanNotBeEmpty"]));

                if (_saveAsDir.IsEmpty())
                    throw new Warning(string.Format("{0}{1}", L["SaveAsPath"], L["CanNotBeEmpty"]));

                if (_saveAsName.IsEmpty())
                    throw new Warning(string.Format("{0}{1}", L["SaveAsName"], L["CanNotBeEmpty"]));

                var saveAsPath = Path.Combine(_saveAsDir, _saveAsName);
                if (saveAsPath.Equals(_currentProjectPath))
                    throw new Warning(string.Format(L["SaveAsPathSameError"], L["ProjectPath"]));

                if (Directory.Exists(saveAsPath))
                    throw new Warning(L["SaveAsPathExist"]);


                var processBarParameters = ProcessBarHelper.CreateProcessBarParameters(async (action) =>
                {
                    var result = await _projectBLL?.CopyAsync(_saveAsDir, _saveAsName, _version);
                });
                await ProcessBarHelper.ShowProcessBarDialogAsync(_dialogService, processBarParameters);
                RaiseRequestClose(new DialogResult(ButtonResult.OK));
            }, async (e) => await DialogService.ShowMessageDialog(e.Message, MessageBoxButton.OK, MessageBoxImage.Warning));
        }

        private void ExecuteCancelCommand()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
        }
    }
}
