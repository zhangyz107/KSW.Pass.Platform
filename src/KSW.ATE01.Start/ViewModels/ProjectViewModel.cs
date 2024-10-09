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
        #region Command
        private DelegateCommand _newProjectCommand;
        public DelegateCommand NewProjectCommand =>
            _newProjectCommand ?? (_newProjectCommand = new DelegateCommand(ExecuteNewProjectCommand));

        private DelegateCommand _selectProjectCommand;
        public DelegateCommand SelectProjectCommand =>
            _selectProjectCommand ?? (_selectProjectCommand = new DelegateCommand(ExecuteSelectProjectCommand));

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

        public ProjectViewModel()
        {
        }


        private void ExecuteNewProjectCommand()
        {
            MessageBox.Show($"触发了{nameof(ExecuteNewProjectCommand)}");
        }

        private void ExecuteSelectProjectCommand()
        {
            MessageBox.Show($"触发了{nameof(ExecuteSelectProjectCommand)}");
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
