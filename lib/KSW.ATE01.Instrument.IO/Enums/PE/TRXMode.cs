using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Enums.PE
{
    public enum TRXMode
    {
        DifferentialDriver = 0,
        SingleEndedDriver_P = 1,
        SingleEndedDriver_N = 2,
        DifferentialReceiver = 3,
        SingleEndedReceiver_P = 4,
        SingleEndedReceiver_N = 5,
        LowPowerPE = 12,
        PMU = 13
    }
}
