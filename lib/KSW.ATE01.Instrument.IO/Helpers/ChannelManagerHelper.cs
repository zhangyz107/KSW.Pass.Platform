using KSW.ATE01.Project.Base.Models;
using KSW.ATE01.Project.Base.Models.TestPlans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Helpers
{
    public class ChannelManagerHelper
    {
        private static Regex _siteRegex = new Regex("^slot([1-9]\\d*|0)+:ch([1-9]\\d*|0)$");

        public static bool IsSiteValid(string siteName)
        {
            var siteNameList = CommonData.Instance.UseSiteName;
            return siteNameList != null && siteNameList.Any() ? siteNameList.Contains(siteName) : false;
        }

        /// <summary>
        /// 通过站点信息获取通道号
        /// </summary>
        public static int GetChannelNumSiteInfo(string site, out int slot)
        {
            var channelNum = 0;
            var siteStr = site.ToLower();
            slot = 0;
            if (_siteRegex.IsMatch(siteStr))
            {
                var match = _siteRegex.Match(siteStr);
                var slotStr = match.Groups[1].Value.Trim();
                int.TryParse(slotStr, out slot);
                var channel = match.Groups[2].Value.Trim();
                int.TryParse(channel, out channelNum);
#if !DEBUG
                if (slot >= 16 || slot < 0)
                    throw new ArgumentOutOfRangeException($"slot{slot}超出范围");

                if (channelNum > 127 || channelNum < 0)
                    throw new ArgumentOutOfRangeException($"ch{channel}超出范围");
#endif
            }

            return channelNum;
        }

        /// <summary>
        /// 通过通道号获取站点信息
        /// </summary>
        public static string GetSiteInfo(int slot, int channel)
        {
            return $"slot{slot}:ch{channel}";
        }
    }
}
