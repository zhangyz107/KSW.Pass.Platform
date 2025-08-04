using KSW.ATE01.Project.Base.Models.TestPlans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Models.Results
{
    /// <summary>
    /// 通道结果模型
    /// </summary>
    public class ChannelResultModel<T> : ResultBaseModel, IChannelResultModel<T>
    {
        private const string _siteNamePattern = @"^Site \d+$";

        /// <summary>
        /// 引脚名
        /// </summary>
        public string PinName { get; set; }

        /// <summary>
        /// 站点信息
        /// </summary>
        public string Site { get; set; }

        /// <summary>
        /// 站点名
        /// </summary>
        public string SiteName { get; set; }

        /// <summary>
        /// 站点编号
        /// </summary>
        public int? SiteNumber { get => GetSiteNumber(); }

        /// <summary>
        /// 站点结果
        /// </summary>
        public T SiteResult { get; set; }

        /// <summary>
        /// 站点测试值
        /// </summary>
        public List<T> SiteTestValues { get; set; }


        private int? GetSiteNumber()
        {
            int? result = null;
            if (string.IsNullOrEmpty(SiteName))
                return result;

            var match = new Regex(_siteNamePattern);
            if (match.IsMatch(SiteName))
            {
                var groups = match.Match(SiteName).Groups;
                if (groups.Count > 1 && int.TryParse(groups[1].Value, out int siteNumber))
                    result = siteNumber;
            }
            return result;
        }

    }
}
