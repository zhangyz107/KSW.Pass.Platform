using KSW.ATE01.Instrument.IO.Enums.Instruments;
using KSW.ATE01.Instrument.IO.Helpers;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.Instruments
{
    public abstract class PE131CommandBase<T> : InstrumentCommandBase<T> where T : new()
    {
        protected override BoardType AllowedBoardType { get => BoardType.PE | BoardType.ALPG; }
    }
}
