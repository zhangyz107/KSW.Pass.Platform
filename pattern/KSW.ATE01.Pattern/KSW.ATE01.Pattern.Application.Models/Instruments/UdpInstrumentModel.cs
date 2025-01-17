using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application.Models.Instruments
{
    public class UdpInstrumentModel : InstrumentInfoModel
    {
        public override byte[] Delimiter => Encoding.UTF8.GetBytes("\r\n");

        public override Encoding StringEncoder => Encoding.UTF8;
    }
}
