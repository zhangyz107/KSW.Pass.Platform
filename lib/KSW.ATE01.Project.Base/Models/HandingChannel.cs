using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models
{
    public class HandingChannel
    {
        /// <summary>
        /// 通道
        /// </summary>
        public int Channel { get; set; }

        /// <summary>
        /// 信号
        /// </summary>
        public int Signal { get; set; }

        /// <summary>
        /// 站点序号
        /// </summary>
        public int ActiveSiteIndex { get; set; }

        /// <summary>
        /// 引脚号
        /// </summary>
        public int HandingPinIndex { get; set; }
    }
}
