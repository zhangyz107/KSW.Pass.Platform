using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Enums.Patterns
{
    public enum StrobeType
    {
        /// <summary>
        /// Edge
        /// </summary>
        [Description("Edge")]
        Edge = 0,

        /// <summary>
        /// Window
        /// </summary>
        [Description("Window")]
        Window =1,
    }
}
