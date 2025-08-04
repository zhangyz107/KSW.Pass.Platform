using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.BLLs.Abstractions.DataLogs
{
    public interface IDataLog
    {
        /// <summary>
        /// 打印测试项结果
        /// </summary>
        void PrintTestItemResult(string directoryPath = null);
    }
}
