using KSW.ATE01.Pattern.Application.Models.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application.Events.Patterns
{
    /// <summary>
    /// 消息通知事件
    /// </summary>
    public class MessageUpdateEvent : PubSubEvent<string>
    {
    }
}
