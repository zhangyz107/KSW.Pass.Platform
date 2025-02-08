using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Models.TestPlans;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.Instruments
{
    public abstract class InstrumentCommandBase<T> where T : class
    {
        protected static readonly Lazy<T> _instance = new Lazy<T>((() => default));

        protected TestPlanModel _testPlan { get => TestPlanHelper.GetLoadedTestPlan(); }

        protected List<ChannelModel> _pinList { get; private set; }

        protected virtual List<ChannelModel> GetPinList(string pinList)
        {
            if (string.IsNullOrEmpty(pinList))
                return null;

            _pinList = PinManagerHelper.GetPinsByNameOrGroupName(_testPlan?.Channel, pinList);
            return _pinList;
        }
    }
}
