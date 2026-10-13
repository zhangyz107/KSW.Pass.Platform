using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KSW.ATE01.Instrument.IO.Enums.PE;

namespace KSW.ATE01.Instrument.IO.BLLs.Abstractions.PE
{
    public interface IPe
    {
        /// <summary>
        /// 设置工作模式
        /// </summary>
        /// <param name="trxMode"></param>
        /// <param name="terMode"></param>
        void SetWorkMode(TRXMode trxMode, TERMode terMode);

        /// <summary>
        /// 设置HS工作电压
        /// </summary>
        /// <param name="voh"></param>
        /// <param name="iout"></param>
        void SetHSWorkVoltage(double voh, double iout);

        /// <summary>
        /// 设置LP工作电压
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <param name="cv"></param>
        void SetLPWorkVoltage(double f0, double f1, double cv);
    }
}
