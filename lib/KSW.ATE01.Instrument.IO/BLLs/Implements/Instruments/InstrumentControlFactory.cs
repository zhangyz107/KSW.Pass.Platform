/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：InstrumentControlFactory.cs
// 功能描述：设备控制工厂
//
// 作者：zhangyingzhong
// 日期：2025/01/21 18:02
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.ATE01.Instrument.IO.Enums.Instruments;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements
{
    /// <summary>
    /// 设备控制工厂
    /// </summary>
    public class InstrumentControlFactory : IInstrumentControlFactory
    {
        private static Dictionary<IOTypeEnum, IInstruentControlService> _instrumentControls = new Dictionary<IOTypeEnum, IInstruentControlService>();

        public IInstruentControlService? GetInstrumentControlService(IOTypeEnum ioType)
        {
            if (_instrumentControls.TryGetValue(ioType, out var control))
                return control;
            else
            {
                RegisterControls();
                if (_instrumentControls.TryGetValue(ioType, out var instruentControl))
                    return instruentControl;
            }
            return null;
        }

        public void RegisterControls()
        {
            var ioTypes = Enum.GetValues<IOTypeEnum>();
            if (ioTypes.Any())
            {
                foreach (var ioType in ioTypes)
                {
                    if (_instrumentControls.ContainsKey(ioType))
                        continue;

                    switch (ioType)
                    {
                        case IOTypeEnum.UDP:
                            _instrumentControls[ioType] = new UdpInstrumentControlService();
                            break;
                        case IOTypeEnum.TCP:
                            break;
                        case IOTypeEnum.COM:
                            break;
                        case IOTypeEnum.UnKnow:
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
