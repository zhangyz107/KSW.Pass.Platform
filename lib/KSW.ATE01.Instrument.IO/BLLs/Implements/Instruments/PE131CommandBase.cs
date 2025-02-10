using KSW.ATE01.Instrument.IO.Helpers;
using KSW.ATE01.Instrument.IO.Models.Instruments;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.Instruments
{
    public abstract class PE131CommandBase<T> : InstrumentCommandBase<T> where T : new ()
    {
        protected IInstruentControlService ControlService { get => InstrumentManagerHelper.GetPE131ControlService(); }

        protected InstrumentBaseModel PE131 { get => InstrumentManagerHelper.GetPE131Info(); }
    }
}
