using KSW.ATE01.Instrument.IO.Enums.Dps;
using KSW.ATE01.Instrument.IO.Enums.Patterns;
using KSW.ATE01.Instrument.IO.Enums.Ppmus;
using KSW.ATE01.Instrument.IO.Models.Results;
using KSW.ATE01.Project.Base.Enums.TestPlans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.BLLs.Abstractions.Dps
{
    public interface IDps
    {
        /// <summary>
        /// 设置驱动和比较器
        /// </summary>
        /// <param name="vil"></param>
        /// <param name="vih"></param>
        /// <param name="vol"></param>
        /// <param name="voh"></param>
        /// <param name="vt"></param>
        /// <param name="hiz">Hiz模式</param>
        /// <param name="isDriver50Ω">false：5Ω,true：50Ω</param>
        void SetDriverAndComparator(double vil, double vih, double vol, double voh, double vt, HizType hiz, bool isDriver50Ω = true);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="period"></param>
        /// <param name="fmt"></param>
        /// <param name="strobeMode"></param>
        /// <param name="d0"></param>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="d3"></param>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        void SetTimingDetail(double period, Timingformat fmt, StrobeModeType strobeMode, double d0, double d1, double d2, double d3, double r0, double r1);

        /// <summary>
        /// 设置Timing
        /// </summary>
        /// <param name="period"></param>
        /// <param name="fmt"></param>
        /// <param name="strobeMode"></param>
        /// <param name="d0"></param>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="d3"></param>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        /// <param name="d_d_d"></param>
        /// <param name="en_d_d"></param>
        /// <param name="den_d_c"></param>
        /// <param name="ca_d_d"></param>
        /// <param name="cb_d_d"></param>
        /// <param name="cab_d_c"></param>
        void SetTimingByPins(byte d_d_d = 0, byte en_d_d = 0, short den_d_c = 0, byte ca_d_d = 0, byte cb_d_d = 0, short cab_d_c = 0);

        /// <summary>
        /// 设置FIMV
        /// </summary>
        /// <param name="iforce"></param>
        /// <param name="isDpsVcc"></param>
        /// <param name="isDpsVee"></param>
        /// <param name="vcl"></param>
        /// <param name="vch"></param>
        /// <param name="gang"></param>
        void SetFIMV(double iforce, bool isDpsVcc, bool isDpsVee, double vcl, double vch, GangType gang);

        /// <summary>
        /// 设置FVMI
        /// </summary>
        /// <param name="vforce"></param>
        /// <param name="isDpsVcc"></param>
        /// <param name="isDpsVee"></param>
        /// <param name="icl"></param>
        /// <param name="ich"></param>
        /// <param name="gang"></param>
        void SetFVMI(double vforce, bool isDpsVcc, bool isDpsVee, double icl, double ich, GangType gang);

        /// <summary>
        /// 设置引脚类型
        /// </summary>
        /// <param name="pinType"></param>
        void SetPinType(PinIOType pinType = PinIOType.inout);

        /// <summary>
        /// 设置引脚初始值
        /// </summary>
        /// <param name="pinType"></param>
        void SetPinInit(PinInitVoltageType initType = PinInitVoltageType.high);

        /// <summary>
        /// 获取MV结果
        /// </summary>
        List<ChannelResultModel<double>> GetMV();

        /// <summary>
        /// 获取MI结果
        /// </summary>
        List<ChannelResultModel<DpsMIResultModel>> GetMI();

        /// <summary>
        /// 获取MT结果
        /// </summary>
        List<ChannelResultModel<double>> GetMT();
    }
}
