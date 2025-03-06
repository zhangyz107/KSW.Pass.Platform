using KSW.ATE01.Instrument.IO.Enums.Instruments;
using KSW.ATE01.Instrument.IO.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.Instruments
{
    public abstract class BackplaneCommandBase<T> : InstrumentCommandBase<T> where T : new()
    {
        protected BoardType BoardType { get => BoardType.Backplane; }

        protected IInstruentControlService ControlService { get => InstrumentManagerHelper.GetControlServiceByBoardType(BoardType); }
    }
}
