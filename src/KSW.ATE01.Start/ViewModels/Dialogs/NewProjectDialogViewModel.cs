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

using KSW.ATE01.Application.BLLs.Abstractions;
using KSW.ATE01.Application.Events.Projects;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Domain.Projects.Core.Enums;
using KSW.Ui;
using Microsoft.Extensions.Logging;
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
        private readonly IContainerExtension _containerProvider;
        private readonly IEventAggregator _eventAggregator;
        private readonly IProjectBLL _projectBLL;
        private ProjectInfoModel _projectInfo;
        private bool _isProjectPathEnable;
        private string _configurationName;
        private string _executeName;

        #endregion

        #region Properties
        public DialogCloseListener RequestClose { get; }

        public string Title => L["NewProject"];

        public Dictionary<TestPlanType, string> TestPlanTypeCbItems => new Dictionary<TestPlanType, string>()
        {
            { TestPlanType.Excel,TestPlanType.Excel.Description()},
            { TestPlanType.Csv,TestPlanType.Csv.Description()}
        };


        public ProjectInfoModel ProjectInfo
        {
            get => _projectInfo;
            private set => SetProperty(ref _projectInfo, value);
        }

        public bool IsProjectPathEnable
        {
            get => _isProjectPathEnable;
            set => SetProperty(ref _isProjectPathEnable, value);
        }

        public string ConfigurationName
        {
            get => _configurationName;
            set => SetProperty(ref _configurationName, value);
        }

        public string ExecuteName
        {
            get => _executeName;
            set => SetProperty(ref _executeName, value);
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

        public NewProjectDialogViewModel(
            IContainerExtension containerProvider,
            IEventAggregator eventAggregator) : base(containerProvider)
        {
            _containerProvider = containerProvider;
            _eventAggregator = eventAggregator;
            _projectBLL = _containerProvider.Resolve<IProjectBLL>();

            _projectInfo = new ProjectInfoModel()
            {
                TestPlanType = TestPlanType.Excel,
            };
            _projectInfo.PropertyChanged += ProjectInfo_PropertyChanged;
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

            if (folderDialog.ShowDialog() == true)
            {
                var folderName = folderDialog.FolderName;
                _projectInfo.ProjectPath = Path.Combine(folderName, _projectInfo.ProjectName);
            }
        }

        private async void ExecuteOKCommand()
        {
            try
            {
                await _projectBLL.CreateProjectAsync(_projectInfo);

                _eventAggregator.GetEvent<ProjectInfoUpdateEvent>().Publish();

                RaiseRequestClose(new DialogResult(ButtonResult.OK));
            }
            catch (Exception ex)
            {
                Log.LogError(ex, ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void ExecuteCancelCommand()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
        }

        private void ProjectInfo_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.IsEmpty() && e.PropertyName.Equals(nameof(ProjectInfoModel.ProjectName)))
            {
                if (!_projectInfo.ProjectName.IsEmpty())
                {
                    IsProjectPathEnable = true;
                    ConfigurationName = _projectInfo.ProjectName + _projectInfo.ConfigurationExtension;
                    ExecuteName = _projectInfo.ProjectName + _projectInfo.ExecuteExtension;
                }
                else
                {
                    IsProjectPathEnable = false;
                    ConfigurationName = string.Empty;
                    ExecuteName = string.Empty;
                }
            }
        }
    }
}
