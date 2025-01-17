using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Domain.Projects.Core.Enums
{
    public enum ModuleType
    {
        /// <summary>
        /// vm_vector
        /// </summary>
        [Description("vm_vector")]
        VM_Vector,

        /// <summary>
        /// lvm_vector
        /// </summary>
        [Description("lvm_vector")]
        LVM_Vector,

        /// <summary>
        /// srm_vector
        /// </summary>
        [Description("srm_vector")]
        SRM_Vector
    }
}
