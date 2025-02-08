using KSW.ATE01.Project.Base.Models.TestPlans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Helpers
{
    /// <summary>
    /// 引脚管理帮助类
    /// </summary>
    public class PinManagerHelper
    {
        public static List<ChannelModel> GetPinsByNameOrGroupName(List<ChannelModel> pins, string name)
        {
            if (pins == null || !pins.Any())
                return null;

            if (string.IsNullOrEmpty(name))
                return null;

            var canFind = true;
            var lowerName = name.ToLower();
            var groups = new List<PinGroupModel>();
            //  1、先查组名
            foreach (var pin in pins)
            {
                if (pin.Groups != null && pin.Groups.Any())
                    groups.AddRange(pin.Groups);
            }
            groups = groups.Distinct().ToList();
            var selectGroupIds = groups.Where(x => x.Name.ToLower().Equals(lowerName)).Select(x => x.Id);
            if (selectGroupIds.Any())
                return pins.Where(x => x.Groups.Any(x => selectGroupIds.Contains(x.Id))).ToList();


            //  2、查引脚名
            return pins.Where(x => x.PinName.ToLower().Equals(lowerName)).ToList();
        }
    }
}
