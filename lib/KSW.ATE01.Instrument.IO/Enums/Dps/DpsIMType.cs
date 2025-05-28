using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Enums.Dps
{
    public enum DpsIMType
    {
        /// <summary>
        /// Hiz
        /// </summary>
        [Description("Hiz")]
        Hiz = 0,

        /// <summary>
        /// IM0
        /// </summary>
        [Description("IM0")]
        IM0 = 1,

        /// <summary>
        /// IM1
        /// </summary>
        [Description("IM1")]
        IM1 = 2,

        /// <summary>
        /// IM2
        /// </summary>
        [Description("IM2")]
        IM2 = 4,

        /// <summary>
        /// IM3
        /// </summary>
        [Description("IM3")]
        IM3 = 8,

        /// <summary>
        /// IM4
        /// </summary>
        [Description("IM4")]
        IM4 = 16,

        /// <summary>
        /// IM5
        /// </summary>
        [Description("IM5")]
        IM5 = 32,
    }
}
