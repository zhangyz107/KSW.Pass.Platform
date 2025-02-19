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
        public static int GetChannelNumBySlot(string site)
        {
            var channelNum = -1;
            var siteStr = site.ToLower();

            if (_siteRegex.IsMatch(siteStr))
            {
                var match = _siteRegex.Match(siteStr);
                var slot = match.Groups[1].Value.Trim();
                if (int.TryParse(slot, out int slotNum) && (slotNum > 4 || slotNum <= 0))
                    throw new ArgumentOutOfRangeException($"slot{slot}超出范围");

                channelNum = GetChannelNumFromSlot(slotNum);

                var channel = match.Groups[2].Value.Trim();
                if (int.TryParse(channel, out int channNum) && (channNum > 32 || channNum < 0))
                    throw new ArgumentOutOfRangeException($"ch{channel}超出范围");

                channelNum += channNum;
            }

            return channelNum;
        }

        private static int GetChannelNumFromSlot(int slot)
        {
            var channelNum = 0;

            switch (slot)
            {
                case 1:
                    channelNum = 0;
                    break;
                case 2:
                    channelNum = 32;
                    break;
                case 3:
                    channelNum = 64;
                    break;
                case 4:
                    channelNum = 96;
                    break;
                default:
                    break;
            }

            return channelNum;
        }
    }
}
