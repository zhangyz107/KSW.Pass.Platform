using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Models.TestPlans;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.Instruments
{
    public abstract class InstrumentCommandBase<T> where T : new()
    {
        protected static readonly Lazy<T> _instance = new Lazy<T>((() => new T()));

        protected TestPlanModel TestPlan { get => TestPlanHelper.GetLoadedTestPlan(); }

        protected List<ChannelModel> PinList { get; private set; }

        public static T Instance {  get => _instance.Value; }

        protected virtual List<ChannelModel> GetPinList(string pinList)
        {
            if (string.IsNullOrEmpty(pinList))
                return null;

            PinList = PinManagerHelper.GetPinsByNameOrGroupName(TestPlan?.Channel, pinList);
            return PinList;
        }
    }
}
