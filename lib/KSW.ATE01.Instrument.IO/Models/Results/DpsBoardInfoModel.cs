using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Models.Results
{
    /// <summary>
    /// 板卡信息模型
    /// </summary>
    public class DpsBoardInfoModel
    {
        public float Vcc { get; set; }

        public float VcceRam { get; set; }

        public float VccrGxb { get; set; }

        public float VcctGxb { get; set; }

        public float TempBoard { get; set; }

        public float VccPt { get; set; }

        public float Vcc12V { get; set; }

        public float TempTsd { get; set; }
    }
}
