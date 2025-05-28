using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.BLLs.Abstractions.Commons
{
    public interface ICommon
    {
        /// <summary>
        /// 设置主触发参数
        /// </summary>
        /// <param name="isBackPlane"></param>
        /// <param name="tiggerNum"></param>
        /// <param name="delay"></param>
        /// <param name="isDebug"></param>
        void SetMasterTriggerParam(bool isBackPlane, int tiggerNum, uint delay, bool isDebug = false);

        /// <summary>
        /// 设置主触发使能
        /// </summary>
        /// <param name="enable"></param>
        void SetMasterTriggerEnable(bool enable);

        /// <summary>
        /// 设置从触发参数
        /// </summary>
        /// <param name="tiggerNum"></param>
        /// <param name="delay"></param>
        void SetSlaveTriggerParam(int tiggerNum, uint delay);

        /// <summary>
        /// 设置从触发使能
        /// </summary>
        /// <param name="enable"></param>
        void SetSlaveTriggerEnable(bool enable);
    }
}
