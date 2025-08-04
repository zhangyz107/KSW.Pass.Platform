using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Results.STDF.Enums
{
    public enum EnumPTRParamFlag : byte
    {
        /// <summary>
        /// 刻度误差
        /// </summary>
        Bit0 = 0x1,

        /// <summary>
        /// 漂移误差
        /// </summary>
        Bit1 = 0x2,

        /// <summary>
        /// 检测到振荡
        /// </summary>
        Bit2 = 0x4,

        /// <summary>
        /// 测量值高于测试上限
        /// </summary>
        Bit3 = 0x8,

        /// <summary>
        /// 测量值低于测试下限
        /// </summary>
        Bit4 = 0x10,

        /// <summary>
        /// 测试通过替代限制
        /// </summary>
        Bit5 = 0x20,

        /// <summary>
        /// 如果结果 = 下限，则结果为“通过”
        /// </summary>
        Bit6 = 0x40,

        /// <summary>
        /// 如果结果 = 上限，则结果为“通过”
        /// </summary>
        Bit7 = 0x80,
    }
}
