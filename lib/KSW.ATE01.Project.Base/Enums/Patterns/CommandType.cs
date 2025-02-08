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

        [Description("repeat")]
        repeat = 1,

        [Description("loop")]
        loop = 2,

        [Description("match")]
        match = 3,

        [Description("call")]
        call = 4,

        [Description("halt")]
        halt = 5,

        [Description("reburst")]
        reburst = 6,

        [Description("endloop")]
        endloop = 7,

        [Description("trig")]
        trig = 8,

        [Description("cpua")]
        cpua = 9,

        [Description("cpuaend")]
        cpuaend = 10,

        [Description("fstart")]
        fstart = 11,

        [Description("fstop")]
        fstop = 12,

        [Description("start")]
        start = 13,

        [Description("clr_flag")]
        clr_flag = 14,

        [Description("loopa")]
        loopa = 15,

        [Description("loopb")]
        loopb = 16,

        [Description("loopc")]
        loopc = 17,

        [Description("enable")]
        enable = 18,

        [Description("set_cpu")]
        set_cpu = 19,

        [Description("poploopa")]
        poploopa = 20,

        [Description("poploopb")]
        poploopb = 21,

        [Description("poploopc")]
        poploopc = 22,

        [Description("poploopc")]
        end_loopa = 23,

        [Description("poploopc")]
        end_loopb = 24,

        [Description("poploopc")]
        end_loopc = 25,

        [Description("return_lvm")]
        return_lvm = 26,

        [Description("jump")]
        jump = 27,
    }
}
