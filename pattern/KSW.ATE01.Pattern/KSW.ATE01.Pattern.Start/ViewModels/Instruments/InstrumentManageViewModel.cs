using KSW.ATE01.Pattern.Application.BLLs.Abstractions.Instruments;
using KSW.ATE01.Pattern.Application.Events.Instruments;
using KSW.ATE01.Pattern.Application.Models.Instruments;
using KSW.ATE01.Pattern.Domain.Instruments.Core.Enums;
using KSW.Helpers;
using KSW.Ui;
using Microsoft.Extensions.Logging;
using System.Windows;
using System.Windows.Media;

namespace KSW.ATE01.Pattern.Start.ViewModels
{
    public class InstrumentManageViewModel : ViewModelBase
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private string _ipAddress;
        private int? _port;
        private int? _localPort;
        private InstrumentInfoModel _instrumentInfo;
        private string _address;
        private string _isConnected;
        private ConnectStateEnum _connectState = ConnectStateEnum.UnKown;
        private SolidColorBrush _colorState = new SolidColorBrush(Colors.Black);
        #endregion

        #region Properties
        /// <summary>
        /// 地址
        /// </summary>
        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }

        /// <summary>
        /// 设备地址
        /// </summary>
        public string IpAddress
        {
            get => _ipAddress;
            set => SetProperty(ref _ipAddress, value);
        }

        /// <summary>
        /// 设备端口号
        /// </summary>
        public int? Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        /// <summary>
        /// 本地端口号
        /// </summary>
        public int? LocalPort
        {
            get => _localPort;
            set => SetProperty(ref _localPort, value);
        }

        /// <summary>
        /// 是否连接
        /// </summary>
        public string IsConnected
        {
            get
            {
                return _isConnected;
            }

            set
            {
                SetProperty(ref _isConnected, value);
            }
        }

        public ConnectStateEnum ConnectState
        {
            get
            {
                return _connectState;
            }

            set
            {
                this.SetProperty(ref _connectState, value);
            }
        }

        /// <summary>
        /// 颜色状态
        /// </summary>
        public SolidColorBrush ColorState
        {
            get
            {
                return _colorState;
            }

            set
            {
                this.SetProperty(ref _colorState, value);
            }
        }
        #endregion

        #region Command
        private DelegateCommand _loadingCommand;
        public DelegateCommand LoadingCommand =>
            _loadingCommand ?? (_loadingCommand = new DelegateCommand(ExecuteLoadingCommandAsync));

        private DelegateCommand _connectCommand;
        public DelegateCommand ConnectCommand =>
            _connectCommand ?? (_connectCommand = new DelegateCommand(ExecuteConnectCommand));

        private DelegateCommand _sendCommand;
        public DelegateCommand SendCommand =>
            _sendCommand ?? (_sendCommand = new DelegateCommand(ExecuteSendCommand));
        #endregion

        public InstrumentManageViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
        }

        private async void ExecuteLoadingCommandAsync()
        {
            var instrumentInfo = new UdpInstrumentModel()
            {
                InstrumentName = "PE131",
                IpAddress = "192.168.0.231",
                Port = 40288,
                LocalPort = 9988,
                ConnectType = IOTypeEnum.UDP,
            };

            _instrumentInfo = instrumentInfo;

            Address = _instrumentInfo?.Address;
            IpAddress = _instrumentInfo?.IpAddress;
            Port = _instrumentInfo?.Port;
            LocalPort = _instrumentInfo?.LocalPort;
        }

        private async void ExecuteConnectCommand()
        {
            try
            {
                var factory = ContainerProvider.Resolve<IInstrumentControlFactory>();
                var control = factory?.GetInstrumentControlService(_instrumentInfo.ConnectType);
                if (control != null)
                {
                    control?.CreateConnect(_instrumentInfo);
                    var isConnected = control?.IsConnected(_instrumentInfo);
                    IsConnected = isConnected == true ? "已连接" : "未连接";
                    ColorState = isConnected == true ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                    ConnectState = isConnected == true ? ConnectStateEnum.Connect : ConnectStateEnum.Disconnect;
                }
                Address = _instrumentInfo.Address;
            }
            catch (Exception e)
            {
                //todo:记录日志 
                await DialogService.ShowMessageDialog(e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                Log.LogError(e, e.Message);
            }
        }

        private void ExecuteSendCommand()
        {
            _eventAggregator.GetEvent<InstrumentSendEvent>().Publish(_instrumentInfo);
        }
    }
}
