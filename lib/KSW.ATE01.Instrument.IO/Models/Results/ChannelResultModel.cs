using KSW.ATE01.Project.Base.Models.TestPlans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Models.Results
{
    /// <summary>
    /// 通道结果模型
    /// </summary>
    public class ChannelResultModel<T> : ResultBaseModel
    {
        /// <summary>
        /// 引脚名
        /// </summary>
        public string PinName { get; set; }

        /// <summary>
        /// 站点信息
        /// </summary>
        public string Site { get; set; }

        /// <summary>
        /// 站点结果
        /// </summary>
        public T SiteResult  { get; set; }

        /// <summary>
        /// 站点测试值
        /// </summary>
        public List<T> SiteTestValues { get; set; }
    }
}
