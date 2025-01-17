using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Domain.Instruments.Core.Enums
{
    /// <summary>
    /// 设备连接的类型
    /// </summary>
    public enum IOTypeEnum
    {
        /// <summary>
        /// Udp连接方式
        /// </summary>
        [Description("UDP")]
        UDP = 1,
        /// <summary>
        /// Tcp连接方式
        /// </summary>
        [Description("TCP")]
        TCP = 2,
        /// <summary>
        /// 串口连接方式
        /// </summary>
        [Description("COM")]
        COM = 3,
        /// <summary>
        /// 未知
        /// </summary>
        [Description("N/A")]
        UnKnow = 4,
    }
}
