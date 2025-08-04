using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Results.STDF.Enums
{
    public enum EnumTSROptionalFlag : byte
    {
        /// <summary>
        /// TEST_MIN 值无效
        /// </summary>
        Bit0 = 0xC9,

        /// <summary>
        /// TEST_MAX 值无效
        /// </summary>
        Bit1 = 0xCA,

        /// <summary>
        /// TEST_TIM 值无效
        /// </summary>
        Bit2 = 0xCC,

        /// <summary>
        /// TEST_SUMS 值无效
        /// </summary>
        Bit4 = 0xD8,

        /// <summary>
        /// TEST_SQRS 值无效
        /// </summary>
        Bit5 = 0xE8,
    }
}
