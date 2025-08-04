using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Results.STDF.Enums
{
    public enum EnumPTRTestFlag : byte
    {
        /// <summary>
        /// 测试期间检测到警报
        /// </summary>
        Bit0 = 0x1,

        /// <summary>
        /// RESULT 字段中的值无效。此设置表示测试已执行，但未获取任何数据记录值。您应该读取 TEST_FLG 的第 6 位和第 7 位来确定测试是通过还是失败
        /// </summary>
        Bit1 = 0x2,

        /// <summary>
        /// 测试结果不可靠
        /// </summary>
        Bit2 = 0x4,

        /// <summary>
        /// 发生超时
        /// </summary>
        Bit3 = 0x8,

        /// <summary>
        /// 测试未执行
        /// </summary>
        Bit4 = 0x10,

        /// <summary>
        /// 测试中止
        /// </summary>
        Bit5 = 0x20,

        /// <summary>
        /// 测试完成，没有通过/失败指示
        /// </summary>
        Bit6 = 0x40,

        /// <summary>
        /// 测试失败
        /// </summary>
        Bit7 = 0x80,
    }
}
