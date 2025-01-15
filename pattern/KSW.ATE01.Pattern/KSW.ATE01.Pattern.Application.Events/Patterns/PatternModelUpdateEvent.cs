using KSW.ATE01.Pattern.Application.Models.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application.Events.Patterns
{
    /// <summary>
    /// 向量模型更新事件
    /// </summary>
    public class PatternModelUpdateEvent : PubSubEvent<PatternModel>
    {
    }
}
