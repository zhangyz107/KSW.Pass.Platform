using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Results.STDF.Enums
{
    public enum EnumPRRInfoFlag : byte
    {
        /// <summary>
        /// 这是一个新部件。其数据设备不会取代任何以前的设备。
        /// </summary>
        None = 0x0,

        /// <summary>
        /// 组成当前序列的 PIR、PTR、MPR 和 PRR 记录（标识为具有相同的 HEAD_NUM 和 SITE_NUM）将取代任何先前具有相同 PART_ID 的记录序列。（重复的部件序列通常表示部件测试错误。） 
        /// </summary>
        Bit0 = 0x1,

        /// <summary>
        /// 组成当前序列的 PIR、PTR、MPR、FTR 和 PRR 记录（标识为具有相同的 HEAD_NUM 和 SITE_NUM）将取代任何先前具有相同 X_COORD 和 Y_COORD 的记录序列。（重复的部件序列通常表示测试错误的部件。）
        /// </summary>
        Bit1 = 0x2,

        /// <summary>
        /// 测试异常结束
        /// </summary>
        Bit2 = 0x4,

        /// <summary>
        /// 部分失败
        /// </summary>
        Bit3 = 0x8,

        /// <summary>
        /// 设备已完成测试，但没有通过/失败指示
        /// </summary>
        Bit4 = 0x10,
    }
}
