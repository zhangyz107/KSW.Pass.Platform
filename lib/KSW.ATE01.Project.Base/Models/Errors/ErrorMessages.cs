using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models.Errors
{
    public class ErrorMessages
    {
        public static DpsPpmuErrors DpsPpmu
        {
            get
            {
                return DpsPpmuErrors.Instance;
            }
        }

        public static IOErrors IO
        {
            get
            {
                return IOErrors.Instance;
            }
        }
    }
}
