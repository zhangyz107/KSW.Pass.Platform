using KSW.ATE01.Instrument.IO.Enums.Ppmus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.BLLs.Abstractions.Ppmus
{
    public interface IPpmu
    {
        /// <summary>
        /// 设置驱动器和比较器
        /// </summary>
        /// <param name="vil">取值范围：-2.56V~+6.09V</param>
        /// <param name="vih">取值范围：-2.56V~+6.09V</param>
        /// <param name="vol">取值范围：-2.56V~+6.09V</param>
        /// <param name="voh">取值范围：-2.56V~+6.09V</param>
        /// <param name="vt">取值范围：-2.56V~+6.09V</param>
        /// <param name="iol">取值范围：0mA~25.5mA</param>
        /// <param name="ioh">取值范围：0mA~25.5mA</param>
        /// <param name="activeLoad">Active Load开关，0:off，1:on</param>
        /// <param name="hiz">Hiz模式，0:hiz，1:vt</param>
        /// <param name="dpc"></param>
        void SetDriverAndComparator(double vil, double vih, double vol, double voh, double vt, double iol, double ioh, bool activeLoad, HizType hiz, byte dpc);

        /// <summary>
        /// 设置FIMV
        /// </summary>
        /// <param name="iMType"></param>
        /// <param name="iforce"></param>
        /// <param name="vcl"></param>
        /// <param name="vch"></param>
        void SetFIMV(IMType iMType, double iforce, double vcl, double vch);

        /// <summary>
        /// 设置FVMI
        /// </summary>
        /// <param name="mIType"></param>
        /// <param name="vforce"></param>
        /// <param name="icl"></param>
        /// <param name="ich"></param>
        void SetFVMI(MIType mIType, double vforce, double icl, double ich);
    }
}
