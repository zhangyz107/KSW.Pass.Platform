using KSW.ATE01.Pattern.Application.Models.Instruments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application.Events.Instruments
{
    /// <summary>
    /// 设备发送事件
    /// </summary>
    public class InstrumentSendEvent : PubSubEvent<InstrumentInfoModel>
    {
    }
}
