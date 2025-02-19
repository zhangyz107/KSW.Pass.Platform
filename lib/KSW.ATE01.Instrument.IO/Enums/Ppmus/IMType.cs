using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Enums.Ppmus
{
    /// <summary>
    /// FIMV类型
    /// </summary>
    public enum IMType
    {
        /// <summary>
        /// Hiz
        /// </summary>
        [Description("Hiz")]
        Hiz = 0,

        /// <summary>
        /// IM0(±4.096 µA，Imax=4.096 µA)
        /// </summary>
        [Description("IM0")]
        IM0 = 1,

        /// <summary>
        /// IM1(±40.96 µA，Imax=40.96 µA)
        /// </summary>
        [Description("IM1")]
        IM1 = 2,

        /// <summary>
        /// IM2(±409.6 µA，Imax=409.6 µA)
        /// </summary>
        [Description("IM2")]
        IM2 = 4,

        /// <summary>
        /// IM3(±4.096 mA，Imax=4.096 mA)
        /// </summary>
        [Description("IM3")]
        IM3 = 8,

        /// <summary>
        /// IM4(±40.96 mA，Imax=40.96 mA)
        /// </summary>
        [Description("IM4")]
        IM4 = 16,
    }
}
