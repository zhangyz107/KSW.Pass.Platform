using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Enums.Patterns
{
    public enum PinInitVoltageType
    {
        /// <summary>
        /// low
        /// </summary>
        [Description("low")]
        low = 0,

        /// <summary>
        /// high
        /// </summary>
        [Description("high")]
        high = 1,

        /// <summary>
        /// Hiz
        /// </summary>
        [Description("Hiz")]
        Hiz = 2,
    }
}
