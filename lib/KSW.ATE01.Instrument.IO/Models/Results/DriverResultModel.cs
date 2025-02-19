using KSW.ATE01.Project.Base.Models.TestPlans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Models.Results
{
    /// <summary>
    /// 驱动器结果模型
    /// </summary>
    public class DriverResultModel
    {
        public double Vil { get; set; }

        public double Vih { get; set; }

        public double Vol { get; set; }

        public double Voh { get; set; }

        public double Vt { get; set; }

        public double Iol { get; set; }

        public double Ioh { get; set; }

        public bool ActiveLoad { get; set; }
    }
}
