using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Results.STDF.Enums
{
    public enum EnumPTROptionalFlag : byte
    {
        /// <summary>
        /// RES_SCAL 值无效。将使用具有此测试编号的第一个 PTR 设置的默认值
        /// </summary>
        Bit0 = 0x1,

        /// <summary>
        /// 保留供将来使用，必须为 1
        /// </summary>
        Bit1 = 0x2,

        /// <summary>
        /// 无规格下限
        /// </summary>
        Bit2 = 0x4,

        /// <summary>
        /// 无规格上限
        /// </summary>
        Bit3 = 0x8,

        /// <summary>
        /// LO_LIMIT 和 LLM_SCAL 无效。将使用此测试编号的第一个 PTR 中为这些字段设置的默认值
        /// </summary>
        Bit4 = 0x10,

        /// <summary>
        /// HI_LIMIT 和 HLM_SCAL 无效。将使用此测试编号的第一个 PTR 中为这些字段设置的默认值
        /// </summary>
        Bit5 = 0x20,

        /// <summary>
        /// 此测试无下限（LO_LIMIT 和 LLM_SCAL 无效）
        /// </summary>
        Bit6 = 0x40,

        /// <summary>
        /// 此测试无上限（HI_LIMIT 和 HLM_SCAL 无效）
        /// </summary>
        Bit7 = 0x80,
    }
}
