using KSW.ATE01.Pattern.Application.BLLs.Abstractions.Instruments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application
{
    public class ApplicationModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var factory = containerProvider?.Resolve<IInstrumentControlFactory>();
            factory.RegisterControlServices();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
