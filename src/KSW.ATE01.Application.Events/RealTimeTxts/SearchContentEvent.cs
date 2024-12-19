using KSW.ATE01.Application.Models.RealTimeTxt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Events.RealTimeTxts
{
    public class SearchContentEvent : PubSubEvent<SearchInfoModel>
    {
        public SearchContentEvent()
        {
        }
    }
}
