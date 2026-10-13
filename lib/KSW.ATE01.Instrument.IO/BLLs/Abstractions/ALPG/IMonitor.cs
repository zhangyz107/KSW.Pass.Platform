using KSW.ATE01.Instrument.IO.Enums.Monitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.BLLs.Abstractions.ALPG
{
    public interface IMonitor
    {

        /// <summary>
        /// 启动监视
        /// </summary>
        /// <param name="monitorObject"></param>
        void StartMonitor(MonitorObject monitorObject);

        /// <summary>
        /// 获取监视数据
        /// </summary>
        /// <param name="address"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        byte[] GetMonitorData(ushort address, ushort length);
    }
}
