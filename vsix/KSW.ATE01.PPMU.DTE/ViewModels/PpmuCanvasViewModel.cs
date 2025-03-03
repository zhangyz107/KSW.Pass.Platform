using KSW.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.PPMU.Start.ViewModels
{
    public class PpmuCanvasViewModel : ViewModelBase
    {
        #region Fields
        private bool _isForceSwitch = false;
        private bool _isDGSSwitch = false;
        private double _result;
        private string _slotChannelTxt;
        private string _resultUnit;
        #endregion

        #region Properties
        public bool IsDGSSwitch
        {
            get => _isDGSSwitch;
            set => SetProperty(ref _isDGSSwitch, value);
        }

        public bool IsForceSwitch
        {
            get => _isForceSwitch;
            set => SetProperty(ref _isForceSwitch, value);
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

        public PpmuCanvasViewModel(IContainerProvider containerProvider) : base(containerProvider)
        {
        }

        private void ExecuteReadMeterCommand()
        {

        }

    }
}
