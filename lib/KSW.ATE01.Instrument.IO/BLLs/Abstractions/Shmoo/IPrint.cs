using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.BLLs.Abstractions.Shmoo
{
    public interface IPrint
    {
        /// <summary>
        /// 打印结果至文本
        /// </summary>
        void Text();

        /// <summary>
        /// 打印结果至Csv
        /// </summary>
        void Csv();
    }
}
