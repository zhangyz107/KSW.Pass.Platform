using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Domain.Projects.Core.Enums
{
    /// <summary>
    /// 主视图类型
    /// </summary>
    public enum MainViewType
    {
        [Description("ProjectView")]
        ProjectView = 1,
        [Description("RunView")]
        RunView = 2,
    }
}
