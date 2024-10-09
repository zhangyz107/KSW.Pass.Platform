using KSW.ATE01.Start.Views;
using KSW.ATE01.Start.Views.Dialogs;
using KSW.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KSW.ATE01.Start.ViewModels
{
    public class ProjectViewModel : ViewModel
    {
        #region Fields
        private readonly IContainerExtension _container;
        private readonly IDialogService _dialogService;
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

        public ProjectViewModel(IContainerExtension container, IDialogService dialogService)
        {
            _container = container;
            _dialogService = dialogService;

            #region 加载页面
            _projectDetailView = _container.Resolve<ProjectDetailView>();
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
            MessageBox.Show($"触发了{nameof(ExecuteSaveAsCommand)}");
        }

        private void ExecuteDelelopCommand()
        {
            MessageBox.Show($"触发了{nameof(ExecuteDelelopCommand)}");
        }
        private void ExecuteRunCommand()
        {
            MessageBox.Show($"触发了{nameof(ExecuteRunCommand)}");
        }

        private void ExecuteReleaseCommand()
        {
            MessageBox.Show($"触发了{nameof(ExecuteReleaseCommand)}");
        }
    }
}
