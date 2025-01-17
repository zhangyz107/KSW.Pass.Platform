using KSW.Application;
using KSW.ATE01.Pattern.Application.BLLs.Abstractions.Instruments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application.BLLs.Implements.Instruments
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandBLL : ServiceBase, ICommandBLL
    {
        public CommandBLL(IContainerProvider containerProvider) : base(containerProvider)
        {
        }
    }
}
