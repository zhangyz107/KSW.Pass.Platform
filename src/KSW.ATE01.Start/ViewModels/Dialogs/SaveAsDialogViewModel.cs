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

using KSW.ATE01.Application.BLLs.Abstractions;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Domain.Projects.Core.Enums;
using KSW.Ui;
using Microsoft.Win32;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Windows;
using KSW.Exceptions;
using KSW.Helpers;
using KSW.ATE01.Application.Events.Projects;

namespace KSW.ATE01.Start.ViewModels.Dialogs
{
    /// <summary>
    /// 另存为窗口视图模型
    /// </summary>
    public class SaveAsDialogViewModel : ViewModelBase, IDialogAware
    {
        #region Fields
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IProjectBLL _projectBLL;
        private TestPlanType? _testPlanType;
        private ProjectInfoModel _currentProjectInfo;
        private string _currentProjectPath;
        private string _saveAsDir;
        private string _saveAsName;

        #endregion

        #region Properties
        public DialogCloseListener RequestClose { get; }
        public string Title => L["SaveAs"];
        public Dictionary<TestPlanType, string> TestPlanTypeCbItems => new Dictionary<TestPlanType, string>()
        {
            { Domain.Projects.Core.Enums.TestPlanType.Excel,Domain.Projects.Core.Enums.TestPlanType.Excel.Description()},
            { Domain.Projects.Core.Enums.TestPlanType.Csv,Domain.Projects.Core.Enums.TestPlanType.Csv.Description()}
        };

        /// <summary>
        /// 测试计划类型
        /// </summary>
        public TestPlanType? TestPlanType
        {
            get => _testPlanType;
            set => SetProperty(ref _testPlanType, value);
        }

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

        public SaveAsDialogViewModel(
            IContainerProvider containerProvider,
            IDialogService dialogService,
            IEventAggregator eventAggregator) : base(containerProvider)
        {
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;
            _projectBLL = ContainerProvider.Resolve<IProjectBLL>();

            LoadData();
        }

        private void LoadData()
        {
            _currentProjectInfo = _projectBLL?.GetCurrentProjectInfo();
            _testPlanType = _currentProjectInfo?.TestPlanType;
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
            try
            {
                if (_currentProjectInfo == null)
                    throw new Warning(string.Format("{0}{1}", L["SelectProject"], L["IsEmpty"]));

                if (_saveAsDir.IsEmpty())
                    throw new Warning(string.Format("{0}{1}", L["SaveAsPath"], L["CanNotBeEmpty"]));

                if (_saveAsName.IsEmpty())
                    throw new Warning(string.Format("{0}{1}", L["SaveAsName"], L["CanNotBeEmpty"]));

                var saveAsPath = Path.Combine(_saveAsDir, _saveAsName);
                if (saveAsPath.Equals(_currentProjectPath))
                    throw new Warning(string.Format(L["SaveAsPathSameError"], L["ProjectPath"]));

                var processBarParameters = ProcessBarHelper.CreateProcessBarParameters(async (action) =>
                {
                    var result = await _projectBLL?.SaveAsProjectInfoAsync(_testPlanType.GetValueOrDefault(), _saveAsDir, _saveAsName);

                    if (result)
                    {
                        _eventAggregator.GetEvent<ProjectInfoUpdateEvent>().Publish();
                        RaiseRequestClose(new DialogResult(ButtonResult.OK));
                    }
                });
                await ProcessBarHelper.ShowProcessBarDialogAsync(_dialogService, processBarParameters);
            }
            catch (Exception e)
            {
                await _dialogService.ShowMessageDialog(e.Message, MessageBoxButton.OK, MessageBoxImage.Warning);
                Log.LogError(e, e.Message);
            }

        }

        private void ExecuteCancelCommand()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
        }
    }
}
