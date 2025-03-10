using KSW.ATE01.Project.Base.Models.TestPlans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Helpers
{
    public class ChannelManagerHelper
    {
        private static Regex _siteRegex = new Regex("^slot([1-9]\\d*|0)+:ch([1-9]\\d*|0)$");

        /// <summary>
        /// 通过站点信息获取通道号
        /// </summary>
        public static int GetChannelNumSiteInfo(string site,out int slot)
        {
            var channelNum = 0;
            var siteStr = site.ToLower();
            slot = 0;
            if (_siteRegex.IsMatch(siteStr))
            {
                var match = _siteRegex.Match(siteStr);
                var slotStr = match.Groups[1].Value.Trim();
                if (int.TryParse(slotStr, out slot) && (slot >= 16 || slot < 0))
                    throw new ArgumentOutOfRangeException($"slot{slot}超出范围");

                var channel = match.Groups[2].Value.Trim();
                if (int.TryParse(channel, out channelNum) && (channelNum > 127 || channelNum < 0))
                    throw new ArgumentOutOfRangeException($"ch{channel}超出范围");
            }

            return channelNum;
        }

        /// <summary>
        /// 通过通道号获取站点信息
        /// </summary>
        public static string GetSlotByChannelNum(int channelNum)
        {
            var result = string.Empty;

            if (channelNum >= 0 && channelNum <= 127)
            {
                var slot = channelNum / 32 + 1;
                var channel = channelNum % 32;
                result = $"slot{slot}:ch{channel}";
            }

            return result;
        }
    }
}
