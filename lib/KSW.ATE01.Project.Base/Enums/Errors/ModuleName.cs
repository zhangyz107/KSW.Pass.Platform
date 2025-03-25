using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Enums.Errors
{
    public enum ModuleName
    {
        /// <summary>
        /// DpsPpmu
        /// </summary>
        [Description("DpsPpmu")]
        DpsPpmu,

        /// <summary>
        /// DPS
        /// </summary>
        [Description("DPS")]
        DPS,

        /// <summary>
        /// IO
        /// </summary>
        [Description("IO")]
        IO,

        /// <summary>
        /// 流程
        /// </summary>
        [Description("Flow")]
        Flow,

        /// <summary>
        /// 一般情况
        /// </summary>
        [Description("InsGeneral")]
        InsGeneral,

        /// <summary>
        /// Shmoo图
        /// </summary>
        [Description("Shmoo")]
        Shmoo
    }
}
