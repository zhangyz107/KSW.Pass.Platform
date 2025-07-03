using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Models.Results
{
    public interface IChannelResultModel<T>
    {
        /// <summary>
        /// 通道号
        /// </summary>
        int ChannelNum { get; set; }

        /// <summary>
        /// 引脚名
        /// </summary>
        string PinName { get; set; }

        /// <summary>
        /// 站点信息
        /// </summary>
        string Site { get; set; }

        /// <summary>
        /// 站点结果
        /// </summary>
        T SiteResult { get; set; }

        /// <summary>
        /// 站点测试值
        /// </summary>
        List<T> SiteTestValues { get; set; }

        /// <summary>
        /// 原始数据
        /// </summary>
        byte[] OriginalData { get; set; }
    }
}
