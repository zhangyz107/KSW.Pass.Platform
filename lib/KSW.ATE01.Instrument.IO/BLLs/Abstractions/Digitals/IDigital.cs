using KSW.ATE01.Instrument.IO.Enums.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.BLLs.Abstractions.Digitals
{
    public interface IDigital
    {
        void SetTimingByPins(sbyte pwa_en = 0, byte cd_en = 0, ushort fd_en = 0, sbyte pwa_d = 0, byte cd_d = 0, ushort fd_d = 0, sbyte pwa_ca = 0, byte cd_ca = 0, ushort fd_ca = 0, sbyte pwa_cb = 0, byte cd_cb = 0, ushort fd_cb = 0, byte d_d_d = 0, byte en_d_d = 0, byte ca_d_d = 0, byte cb_d_d = 0, short cab_d_c = 0);
    }
}
