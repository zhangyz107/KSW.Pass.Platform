using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Enums.Dps
{
    /// <summary>
    /// FIMV类型
    /// </summary>
    public enum IRType
    {
        /// <summary>
        /// Hiz
        /// </summary>
        [Description("Hiz")]
        Hiz = 0,

        /// <summary>
        /// IR0
        /// </summary>   
        [Description("IR0")]
        IR0 = 1,

        /// <summary>
        /// IR1
        /// </summary>     
        [Description("IR1")]
        IR1 = 2,

        /// <summary>
        /// IR2
        /// </summary>    
        [Description("IR2")]
        IR2 = 4,

        /// <summary>
        /// IR3
        /// </summary>    
        [Description("IR3")]
        IR3 = 8,

        /// <summary>
        /// IR4
        /// </summary>     
        [Description("IR4")]
        IR4 = 16,

        /// <summary>
        /// IR5
        /// </summary>     
        [Description("IR5")]
        IR5 = 32
    }
}
