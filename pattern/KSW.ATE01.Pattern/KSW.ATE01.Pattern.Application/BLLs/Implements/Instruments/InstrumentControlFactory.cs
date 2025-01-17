using KSW.Application;
using KSW.ATE01.Pattern.Application.BLLs.Abstractions.Instruments;
using KSW.ATE01.Pattern.Domain.Instruments.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application.BLLs.Implements.Instruments
{
    public class InstrumentControlFactory : ServiceBase, IInstrumentControlFactory
    {
        private readonly IContainerExtension _container;

        public InstrumentControlFactory(IContainerExtension container) : base(container)
        {
            _container = container;
        }

        public IInstruentControlService GetInstrumentControlService(IOTypeEnum ioType)
        {
            return _container.Resolve<IInstruentControlService>(ioType.ToString());
        }

        public void RegisterControlServices()
        {
            foreach (var item in Enum.GetValues<IOTypeEnum>())
            {
                switch (item)
                {
                    case IOTypeEnum.UDP:
                        _container.RegisterSingleton(typeof(IInstruentControlService), typeof(UdpInstrumentControlService), item.ToString());
                        break;
                    case IOTypeEnum.TCP:
                        break;
                    case IOTypeEnum.COM:
                        break;
                    case IOTypeEnum.UnKnow:
                        break;
                }
            }
        }
    }
}
