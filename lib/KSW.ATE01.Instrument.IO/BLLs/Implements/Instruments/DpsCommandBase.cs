using KSW.ATE01.Instrument.IO.Enums.Instruments;
using KSW.ATE01.Instrument.IO.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.Instruments
{
    public abstract class DpsCommandBase<T> : InstrumentCommandBase<T> where T : new()
    {
        protected override BoardType BoardType { get => BoardType.DPS; }
    }
}
