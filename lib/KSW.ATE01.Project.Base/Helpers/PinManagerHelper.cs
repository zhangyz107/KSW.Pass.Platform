using KSW.ATE01.Project.Base.Models.TestPlans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KSW.ATE01.Project.Base.Helpers
{
    /// <summary>
    /// 引脚管理帮助类
    /// </summary>
    public class PinManagerHelper
    {
        public static int GetPinIndexByPinName(List<ChannelModel> pins, string name)
        {
            var result = -1;
            if (pins == null || !pins.Any())
                return result;

            if (string.IsNullOrEmpty(name))
                return result;

            var channel = GetPinByName(pins, name);
            return pins.IndexOf(channel);
        }

        public static ChannelModel GetPinByName(List<ChannelModel> pins, string name)
        {
            if (pins == null || !pins.Any())
                return null;

            if (string.IsNullOrEmpty(name))
                return null;

            var lowerName = name.ToLower();

            //  查引脚名
            return pins.Where(x => x.PinName.ToLower().Equals(lowerName)).FirstOrDefault();
        }

        public static List<ChannelModel> GetPinsByNameOrGroupName(List<ChannelModel> pins, string name)
        {
            if (pins == null || !pins.Any())
                return null;

            if (string.IsNullOrEmpty(name))
                return null;

            var result = new List<ChannelModel>();
            var groups = new List<PinGroupModel>();
            var list = name.Split(',').ToList();
            if (list != null && list.Any())
            {
                foreach (var pinStr in list)
                {
                    var lowerName = pinStr.ToLower();
                    //  1、先查组名
                    foreach (var pin in pins)
                    {
                        if (pin.Groups != null && pin.Groups.Any())
                            groups.AddRange(pin.Groups);
                    }
                    groups = groups.Distinct().ToList();
                    var selectGroupIds = groups.Where(x => x.Name.ToLower().Equals(lowerName)).Select(x => x.Id);
                    if (selectGroupIds.Any())
                        result.AddRange(pins.Where(x => x.Groups.Any(x => selectGroupIds.Contains(x.Id))));

                    //  2、查引脚名
                    result.AddRange(pins.Where(x => x.PinName.ToLower().Equals(lowerName)));
                }
            }
            else
            {
                var lowerName = name.ToLower();
                //  1、先查组名
                foreach (var pin in pins)
                {
                    if (pin.Groups != null && pin.Groups.Any())
                        groups.AddRange(pin.Groups);
                }
                groups = groups.Distinct().ToList();
                var selectGroupIds = groups.Where(x => x.Name.ToLower().Equals(lowerName)).Select(x => x.Id);
                if (selectGroupIds.Any())
                    result.AddRange(pins.Where(x => x.Groups.Any(x => selectGroupIds.Contains(x.Id))));

                //  2、查引脚名
                result.AddRange(pins.Where(x => x.PinName.ToLower().Equals(lowerName)));
            }
            return result;
        }

        public static string GetPinNameBySlotName(List<ChannelModel> pins, string slot)
        {
            string result = null;
            if (pins == null || !pins.Any())
                return null;

            if (string.IsNullOrEmpty(slot))
                return null;

            var lowerSlot = slot.ToLower();
            foreach (var pin in pins)
            {
                if (pin.Sites.Any())
                {
                    foreach (var site in pin.Sites)
                    {
                        if (site.SiteValue.ToLower().Equals(lowerSlot))
                        {
                            result = pin.PinName;
                            return result;
                        }
                    }
                }
            }
            return result;
        }
    }
}
