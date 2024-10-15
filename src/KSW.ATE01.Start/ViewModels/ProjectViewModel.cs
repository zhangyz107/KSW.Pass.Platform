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
//
//------------------------------------------------------------*/

using KSW.ATE01.Application.BLLs.Abstractions;
using KSW.ATE01.Start.Views;
using KSW.ATE01.Start.Views.Dialogs;
using KSW.Helpers;
using KSW.Ui;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace KSW.ATE01.Start.ViewModels
{
    /// <summary>
    /// 项目视图模型
    /// </summary>
    public class ProjectViewModel : ViewModelBase
    {
        #region Fields
        private readonly IContainerExtension _containerProvider;
        private readonly IDialogService _dialogService;
        private readonly IProjectBLL _projectBLL;
        private ProjectDetailView _projectDetailView;
        #endregion

        #region Properties

        public ProjectDetailView ProjectDetailView
        {
            get => _projectDetailView;
            set => SetProperty(ref _projectDetailView, value);
        }

        #endregion

        #region Command
        private DelegateCommand _newProjectCommand;
        public DelegateCommand NewProjectCommand =>
            _newProjectCommand ?? (_newProjectCommand = new DelegateCommand(ExecuteNewProjectCommand));

        private DelegateCommand _openProjectCommand;
        public DelegateCommand OpenProjectCommand =>
            _openProjectCommand ?? (_openProjectCommand = new DelegateCommand(ExecuteOpenProjectCommand));

        private DelegateCommand _saveAsCommand;
        public DelegateCommand SaveAsCommand =>
            _saveAsCommand ?? (_saveAsCommand = new DelegateCommand(ExecuteSaveAsCommand));

        private DelegateCommand _delelopCommand;
        public DelegateCommand DelelopCommand =>
            _delelopCommand ?? (_delelopCommand = new DelegateCommand(ExecuteDelelopCommand));

        private DelegateCommand _runCommand;
        public DelegateCommand RunCommand =>
            _runCommand ?? (_runCommand = new DelegateCommand(ExecuteRunCommand));

        private DelegateCommand _releaseCommand;
        public DelegateCommand ReleaseCommand =>
            _releaseCommand ?? (_releaseCommand = new DelegateCommand(ExecuteReleaseCommand));

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public ProjectViewModel(
            IContainerExtension containerProvider,
            IDialogService dialogService) : base(containerProvider)
        {
            _containerProvider = containerProvider;
            _dialogService = dialogService;
            _projectBLL = containerProvider.Resolve<IProjectBLL>() ?? throw new ArgumentNullException(nameof(IProjectBLL));
            #region 加载页面
            _projectDetailView = _containerProvider.Resolve<ProjectDetailView>();
            #endregion
        }


        private void ExecuteNewProjectCommand()
        {
            _dialogService.ShowDialog(nameof(NewProjectDialog));
        }

        private void ExecuteOpenProjectCommand()
        {
            _dialogService.ShowDialog(nameof(OpenProjectDialog));
        }
        private void ExecuteSaveAsCommand()
        {
            _dialogService.ShowDialog(nameof(SaveAsDialog));
        }

        private void ExecuteDelelopCommand()
        {
            try
            {
                _projectBLL.RunProjecctByVS();
            }
            catch (Exception e)
            {
                _dialogService.ShowMessageDialog(e.Message);
                Log?.LogError(e, e.Message);
            }
        }
        private void ExecuteRunCommand()
        {
            _dialogService.ShowDialog(nameof(RunDialog));
        }

        private void ExecuteReleaseCommand()
        {
            _dialogService.ShowDialog(nameof(ReleaseDialog));
        }
    }
}
