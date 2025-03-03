using KSW.Helpers;
using KSW.Ui;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.PPMU.Start.ViewModels
{
    /// <summary>
    /// 主窗口视图模型
    /// </summary>
    public class ShellViewModel : ViewModelBase
    {
        public string Title { get => "PPMU DTE"; }
        private string _voltage;

        #region Properties
        public string Voltage
        {
            get => _voltage;
            set => SetProperty(ref _voltage, value);
        }

        #endregion

        #region Commands
        private DelegateCommand _refreshCommand;
        public DelegateCommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new DelegateCommand(ExecuteRefreshCommand));
        #endregion

        public ShellViewModel(IContainerProvider containerProvider) : base(containerProvider)
        {

        }


        private void ExecuteRefreshCommand()
        {
            DialogService.ShowMessageDialog("Test");
        }
    }
}
