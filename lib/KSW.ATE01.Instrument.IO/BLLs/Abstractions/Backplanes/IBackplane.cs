using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.BLLs.Abstractions.Backplanes
{
    public interface IBackplane
    {
        public void SetChannelEnable(bool enable);
    }
}
