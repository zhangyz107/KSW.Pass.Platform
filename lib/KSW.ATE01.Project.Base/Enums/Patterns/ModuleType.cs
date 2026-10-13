using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Enums.Patterns
{
    public enum ModuleType
    {
        [Description("VM_Vector")]
        VM_Vector,

        [Description("LVM_Vector")]
        LVM_Vector,

        [Description("SRM_Vector")]
        SRM_Vector
    }
}
