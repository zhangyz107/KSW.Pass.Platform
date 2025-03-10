using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Enums.Patterns
{
    /// <summary>
    /// 命令类型
    /// </summary>
    public enum CommandType
    {
        [Description("nop")]
        nop = 0,

        [Description("loop")]
        loop = 0x1,

        [Description("stop")]
        stop = 0x2,

        [Description("repeat")]
        repeat = 3,

        [Description("endloop")]
        endloop = 0x4,

        [Description("match")]
        match = 5,
        
        [Description("call")]
        call = 6,

        [Description("halt")]
        halt = 7,

        [Description("reburst")]
        reburst = 8,

        [Description("trig")]
        trig = 9,

        [Description("cpua")]
        cpua = 10,

        [Description("cpuaend")]
        cpuaend = 11,

        [Description("fstart")]
        fstart = 12,

        [Description("fstop")]
        fstop = 13,

        [Description("start")]
        start = 14,

        [Description("clr_flag")]
        clr_flag = 15,

        [Description("loopa")]
        loopa = 16,

        [Description("loopb")]
        loopb = 17,

        [Description("loopc")]
        loopc = 18,

        [Description("enable")]
        enable = 19,

        [Description("set_cpu")]
        set_cpu = 20,

        [Description("poploopa")]
        poploopa = 21,

        [Description("poploopb")]
        poploopb = 22,

        [Description("poploopc")]
        poploopc = 23,

        [Description("poploopc")]
        end_loopa = 24,

        [Description("poploopc")]
        end_loopb = 25,

        [Description("poploopc")]
        end_loopc = 26,

        [Description("return_lvm")]
        return_lvm = 27,

        [Description("jump")]
        jump = 28,
    }
}
