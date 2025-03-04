using KSW.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.DPS.Start.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        public string Title { get => "DPS DTE"; }


        #region Commands
        private DelegateCommand _refreshCommand;
        public DelegateCommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new DelegateCommand(ExecuteRefreshCommand));

        private DelegateCommand _clearAlarmCommand;
        public DelegateCommand ClearAlarmCommand => _clearAlarmCommand ?? (_clearAlarmCommand = new DelegateCommand(ExecuteClearAlarmCommand));
        #endregion

        public ShellViewModel(IContainerProvider containerProvider) : base(containerProvider)
        {

        }

        private void ExecuteRefreshCommand()
        {

        }

        private void ExecuteClearAlarmCommand()
        {

        }
    }
}
