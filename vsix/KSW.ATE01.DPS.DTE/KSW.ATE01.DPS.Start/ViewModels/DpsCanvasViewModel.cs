using KSW.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.DPS.Start.ViewModels
{
    public class DpsCanvasViewModel : ViewModelBase
    {
        private bool _isConnect = false;
        private double _result;
        private string _slotChannelTxt;
        private string _resultUnit;

        #region Properties
        public bool IsConnect
        {
            get => _isConnect;
            set => SetProperty(ref _isConnect, value);
        }

        public double Result
        {
            get => _result;
            set => SetProperty(ref _result, value);
        }

        public string ResultUnit
        {
            get => _resultUnit;
            set => SetProperty(ref _resultUnit, value);
        }

        public string SlotChannelTxt
        {
            get => _slotChannelTxt;
            set => SetProperty(ref _slotChannelTxt, value);
        }
        #endregion

        #region Command
        private DelegateCommand _readMeterCommand;
        public DelegateCommand ReadMeterCommand => _readMeterCommand ?? (_readMeterCommand = new DelegateCommand(ExecuteReadMeterCommand));
        #endregion

        public DpsCanvasViewModel(IContainerProvider containerProvider) : base(containerProvider)
        {

        }

        private void ExecuteReadMeterCommand()
        {

        }
    }
}
