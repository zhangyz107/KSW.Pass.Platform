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
        /// IR0
        /// </summary>   
        [Description("IR0")]
        IR0 = 0,

        /// <summary>
        /// IR1
        /// </summary>     
        [Description("IR1")]
        IR1 = 1,

        /// <summary>
        /// IR2
        /// </summary>    
        [Description("IR2")]
        IR2 = 2,

        /// <summary>
        /// IR3
        /// </summary>    
        [Description("IR3")]
        IR3 = 4,

        /// <summary>
        /// IR4
        /// </summary>     
        [Description("IR4")]
        IR4 = 8,

        /// <summary>
        /// IR5
        /// </summary>     
        [Description("IR5")]
        IR5 = 16
    }
}
