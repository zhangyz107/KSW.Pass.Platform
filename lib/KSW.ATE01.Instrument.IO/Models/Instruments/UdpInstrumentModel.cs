using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Models.Instruments
{
    public class UdpInstrumentModel : InstrumentBaseModel
    {
        public override byte[] Delimiter => Encoding.UTF8.GetBytes("\r\n");

        public override Encoding StringEncoder => Encoding.UTF8;
    }
}
