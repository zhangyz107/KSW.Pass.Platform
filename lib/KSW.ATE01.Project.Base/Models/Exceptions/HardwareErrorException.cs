using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models.Exceptions
{
    public class HardwareErrorException : Exception
    {
        /// <summary>
        /// 机箱
        /// </summary>
        public int Chassis { get; set; }

        /// <summary>
        /// 插槽
        /// </summary>
        public int Slot { get; set; }

        /// <summary>
        /// 通道信息
        /// </summary>
        public HandingChannel Channel { get; set; }

        /// <summary>
        /// 本地化
        /// </summary>
        public string Location { get; set; }
    }
}
