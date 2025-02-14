using KSW.ATE01.Project.Base.Enums.Errors;
using KSW.ATE01.Project.Base.Enums.Patterns;
using KSW.ATE01.Project.Base.Services.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models.Errors
{
    public class DpsPpmuErrors
    {
        private static readonly Lazy<DpsPpmuErrors> _lazy = new Lazy<DpsPpmuErrors>(() => new DpsPpmuErrors());

        public static DpsPpmuErrors Instance
        {
            get { return _lazy.Value; }
        }

        public ErrorAgent Agent { get;private set; }

        public DpsPpmuErrors()
        {
            InitError();
        }

        private void InitError()
        {
            Instance.Agent = new ErrorAgent(ModuleName.DpsPpmu.ToString());
        }
    }
}
