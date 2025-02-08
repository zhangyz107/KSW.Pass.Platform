using KSW.ATE01.Instrument.IO.BLLs.Implements;
using KSW.ATE01.Instrument.IO.Enums.Instruments;
using KSW.ATE01.Instrument.IO.Models.Instruments;

namespace KSW.ATE01.Instrument.IO.Helpers
{
    public class InstrumentManagerHelper
    {
        private readonly IInstrumentControlFactory _controlFactory;
        private readonly Dictionary<string,UdpInstrumentModel> _instrumentDic = new Dictionary<string,UdpInstrumentModel>();
        private static readonly Lazy<InstrumentManagerHelper> _instance = new Lazy<InstrumentManagerHelper>(() => new InstrumentManagerHelper());

        public InstrumentManagerHelper()
        {
            _controlFactory = new InstrumentControlFactory();
            _controlFactory.RegisterControls();

            InitInstrumentDic();
        }

        private void InitInstrumentDic()
        {
            _instrumentDic.Add("PE131", new UdpInstrumentModel()
            {
                InstrumentName = "PE131",
                IpAddress = "192.168.0.231",
                Port = 40288,
                LocalPort = 9988,
                ConnectType = IOTypeEnum.UDP,
            });
        }

        public static IInstruentControlService GetPE131ControlService()
        {
            var helper = _instance.Value;
            if (helper == null)
                return null;

            var instrumentInfo = helper._instrumentDic["PE131"];

            var control = helper._controlFactory.GetInstrumentControlService(instrumentInfo.ConnectType);
            if (control == null)
                return null;

            control?.CreateConnect(instrumentInfo);
            return control;
        }

        public static InstrumentBaseModel GetPE131Info()
        {
            var helper = _instance.Value;
            if (helper == null)
                return null;

            return helper._instrumentDic.ContainsKey("PE131") ? helper._instrumentDic["PE131"] : null;
        }
    }
}
