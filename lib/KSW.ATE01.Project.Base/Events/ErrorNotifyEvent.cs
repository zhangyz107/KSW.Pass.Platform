using KSW.ATE01.Project.Base.Models.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Events
{
    public class ErrorNotifyEvent : PubSubEvent<ErrorMessage>
    {
    }
}
