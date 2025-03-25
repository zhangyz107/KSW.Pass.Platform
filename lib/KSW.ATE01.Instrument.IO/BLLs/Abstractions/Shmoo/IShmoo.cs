using KSW.ATE01.Instrument.IO.BLLs.Implements.Shmoo;
using KSW.ATE01.Instrument.IO.Enums.Shmoos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.BLLs.Abstractions.Shmoo
{
    public interface IShmoo
    {
        IPrint Print { get; }

        void SetX(string type, string mode, double begin, double end, double stepSize, string pinList, string timingName = "", double delayS = 0);

        void SetY(string type, string mode, double begin, double end, double stepSize, string pinList, string timingName = "", double delayS = 0);

        void Mode(AxisDirection axisType);

        void Run(ActiveMode activeMode = ActiveMode.Normal,string patternName = "");
    }
}
