using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Enums.Ppmus
{
    /// <summary>
    /// FVMI类型
    /// </summary>
    public enum MIType
    {
        /// <summary>
        /// IR0(±4.096 µA，Imax=4.096 µA)
        /// </summary>
        [Description("IR0")]
        IR0 = 0,

        /// <summary>
        /// IR1(±40.96 µA，Imax=40.96 µA)
        /// </summary>
        [Description("IR1")]
        IR1 = 1,

        /// <summary>
        /// IR2(±4096. µA，Imax=4096. µA)
        /// </summary>
        [Description("IR2")]
        IR2 = 2,

        /// <summary>
        /// IR3(±4.096 mA，Imax=4.096 mA)
        /// </summary>
        [Description("IR3")]
        IR3 = 3,

        /// <summary>
        /// IR4(±40.96 mA，Imax=40.96 mA)
        /// </summary>
        [Description("IR4")]
        IR4 = 4,
    }
}
