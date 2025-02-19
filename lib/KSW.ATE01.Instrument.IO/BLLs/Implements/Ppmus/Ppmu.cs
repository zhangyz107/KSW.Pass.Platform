using KSW.ATE01.Instrument.IO.BLLs.Abstractions.Ppmus;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Instruments;
using KSW.ATE01.Instrument.IO.Enums.Instruments;
using KSW.ATE01.Instrument.IO.Enums.Ppmus;
using KSW.ATE01.Instrument.IO.Helpers;
using KSW.ATE01.Instrument.IO.Models.Instruments;
using KSW.ATE01.Instrument.IO.Models.Results;
using System.Runtime.Intrinsics.Arm;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.Ppmus
{
    public class Ppmu : PE131CommandBase<Ppmu>, IPpmu
    {
        private PpmuCurrentRange _currentRange;
        private double _iforce;
        private double _vcl;
        private double _vch;
        private IMType _imType;

        #region Properties
        public PpmuCurrentRange CurrentRange
        {
            get
            {
                if (_currentRange == null)
                    _currentRange = new PpmuCurrentRange(this);

                _currentRange._imType = _imType;
                _currentRange._iforce = _iforce;
                _currentRange._vcl = _vcl;
                _currentRange._vch = _vch;

                return _currentRange;
            }
        }
        #endregion

        public static IPpmu Pins(string pinList)
        {
            if (string.IsNullOrEmpty(pinList))
                throw new ArgumentNullException(nameof(pinList));

            Instance.GetPinList(pinList);

            return Instance;
        }

        public void SetDriverAndComparator(double vil, double vih, double vol, double voh, double vt, double iol, double ioh, bool activeLoad, HizType hiz, byte dpc)
        {
            if (PinList == null || !PinList.Any())
                return;

            try
            {
                if (vil < -2.56 || vil > 6.09)
                    throw new ArgumentOutOfRangeException(nameof(vil), "取值范围:-2.56V~+6.09V");

                if (vih < -2.56 || vih > 6.09)
                    throw new ArgumentOutOfRangeException(nameof(vih), "取值范围:-2.56V~+6.09V");

                if (vol < -2.56 || vol > 6.09)
                    throw new ArgumentOutOfRangeException(nameof(vol), "取值范围:-2.56V~+6.09V");

                if (voh < -2.56 || voh > 6.09)
                    throw new ArgumentOutOfRangeException(nameof(voh), "取值范围:-2.56V~+6.09V");

                if (vt < -2.56 || vt > 6.09)
                    throw new ArgumentOutOfRangeException(nameof(vt), "取值范围:-2.56V~+6.09V");

                if (iol < 0 || iol > 25.5)
                    throw new ArgumentOutOfRangeException(nameof(iol), "取值范围:0mA~25.5mA");

                if (ioh < 0 || ioh > 25.5)
                    throw new ArgumentOutOfRangeException(nameof(ioh), "取值范围:0mA~25.5mA");

                // 转换数据格式
                var vilValue = GetUshortValue(vil);
                var vilBytes = BitConverter.GetBytes(vilValue).Reverse();
                var vihValue = GetUshortValue(vih);
                var vihBytes = BitConverter.GetBytes(vihValue).Reverse();
                var volValue = GetUshortValue(vol);
                var volBytes = BitConverter.GetBytes(volValue).Reverse();
                var vohValue = GetUshortValue(voh);
                var vohBytes = BitConverter.GetBytes(vohValue).Reverse();
                var vtValue = GetUshortValue(vt);
                var vtBytes = BitConverter.GetBytes(vtValue).Reverse();
                var iolByte = GetByteValue(iol);
                var iohByte = GetByteValue(ioh);

                //  组装数据包
                var commandList = new List<CommandInfoModel>();
                foreach (var pin in PinList)
                {
                    foreach (var site in pin.Sites)
                    {
                        var channelNum = ChannelManagerHelper.GetChannelNumBySlot(site.SiteValue);
                        if (channelNum > 0)
                        {
                            var byteList = new List<byte>();
                            byteList.Add((byte)channelNum);    //暂时按顺序下发通道（后续需要映射站点信息）
                            byteList.AddRange(vilBytes);  //vil
                            byteList.AddRange(vihBytes);  //vih
                            byteList.AddRange(volBytes);  //vol
                            byteList.AddRange(vohBytes);  //voh
                            byteList.AddRange(vtBytes);  //vt
                            byteList.Add(iolByte);  //iol
                            byteList.Add(iohByte);  //ioh
                            byteList.Add(System.Convert.ToByte(activeLoad));  //Active Load开关，0:off，1:on
                            byteList.Add((byte)hiz);  //Hiz模式，0:hiz，1:vt
                            byteList.Add(dpc);  //DPC

                            var command = new CommandInfoModel()
                            {
                                CommandCode = "0x0100",
                                CommandContent = byteList.ToArray(),
                            };
                            commandList.Add(command);
                        }
                    }
                }

                if (commandList.Any())
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, BoradType.PE, InstructionType.Configuration, commandList);
                    if (ControlService != null && message.Any())
                        ControlService.Send(PE131, message);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private ushort GetUshortValue(double v)
        {
            var tempValue = (v + 2.56) / 132 * 1000000;
            return System.Convert.ToUInt16(Math.Floor(tempValue));
        }

        private byte GetByteValue(double v)
        {
            var tempValue = v / 0.1;
            return System.Convert.ToByte(Math.Floor(tempValue));
        }

        internal void SetFIMV(IMType iMType, double iforce, double vcl, double vch)
        {
            if (PinList == null || !PinList.Any())
                return;
            try
            {
                if (vcl < -2.56 || vcl > 6.06)
                    throw new ArgumentOutOfRangeException(nameof(vcl), "取值范围:-2.56V~+6.06V");

                if (vch < -2.56 || vch > 6.06)
                    throw new ArgumentOutOfRangeException(nameof(vch), "取值范围:-2.56V~+6.06V");

                var imax = GetImaxFromType(iMType);
                var iforceValue = GetIforceUshortValue(iforce, imax);
                var iforceBytes = BitConverter.GetBytes(iforceValue).Reverse();
                var vclByte = GetVclOrVchValue(vcl);
                var vchByte = GetVclOrVchValue(vch);

                //  组装数据包
                var commandList = new List<CommandInfoModel>();
                foreach (var pin in PinList)
                {
                    foreach (var site in pin.Sites)
                    {
                        var channelNum = ChannelManagerHelper.GetChannelNumBySlot(site.SiteValue);
                        if (channelNum > 0)
                        {
                            var byteList = new List<byte>();
                            byteList.Add((byte)channelNum);       //暂时按顺序下发通道（后续需要映射站点信息）
                            byteList.Add((byte)iMType);      //IM
                            byteList.AddRange(iforceBytes);  //iforce
                            byteList.Add(vclByte);           //vcl
                            byteList.Add(vchByte);           //vch

                            var command = new CommandInfoModel()
                            {
                                CommandCode = "0x0101",
                                CommandContent = byteList.ToArray(),
                            };
                            commandList.Add(command);
                        }
                    }
                }

                if (commandList.Any())
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, BoradType.PE, InstructionType.Configuration, commandList);
                    if (ControlService != null && message.Any())
                        ControlService.Send(PE131, message);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetFIMV(double iforce, double vcl, double vch)
        {
            _iforce = iforce;
            _vcl = vcl;
            _vch = vch;

            var iMType = IMType.Hiz;

            var currentAbs = Math.Abs(iforce);
            // 转换数据格式
            if (currentAbs <= 4.096e-6)
            {
                iMType = IMType.IM0;
            }
            else if (4.096e-6 < currentAbs && currentAbs <= 40.96e-6)
            {
                iMType = IMType.IM1;
            }
            else if (40.96e-6 < currentAbs && currentAbs <= 409.6e-6)
            {
                iMType = IMType.IM2;
            }
            else if (409.6e-6 < currentAbs && currentAbs <= 4.096e-3)
            {
                iMType = IMType.IM3;
            }
            else if (4.096e-3 < currentAbs && currentAbs <= 40.96e-3)
            {
                iMType = IMType.IM4;
            }

            _imType = iMType;

            SetFIMV(iMType, iforce, vcl, vch);
        }

        private double GetImaxFromType(IMType type)
        {
            double imax = 0;
            switch (type)
            {
                case IMType.IM0:
                    imax = 4.096e-6;
                    break;
                case IMType.IM1:
                    imax = 40.96e-6;
                    break;
                case IMType.IM2:
                    imax = 409.6e-6;
                    break;
                case IMType.IM3:
                    imax = 4.096e-3;
                    break;
                case IMType.IM4:
                    imax = 40.96e-3;
                    break;
            }
            return imax;
        }

        private ushort GetIforceUshortValue(double i, double imax)
        {
            var tempValue = imax == 0 ? 0 : (i / imax * 1.28 + 1.75 + 2.56) / 132 * 1000000;
            return System.Convert.ToUInt16(Math.Floor(tempValue));
        }

        private byte GetVclOrVchValue(double v)
        {
            var tempValue = (v + 2.56) * 1000 / 33.8;
            return System.Convert.ToByte(Math.Floor(tempValue));
        }

        public void SetFVMI(double vforce, double icl, double ich)
        {
            if (PinList == null || !PinList.Any())
                return;

            var miType = MIType.IR0;
            try
            {
                if (vforce < -1 || vforce > 5)
                    throw new ArgumentOutOfRangeException(nameof(vforce), "取值范围:-1V~+5V");

                var currentAbsMax = Math.Max(Math.Abs(icl), Math.Abs(ich));
                // 转换数据格式
                if (currentAbsMax <= 4.096e-6)
                {
                    miType = MIType.IR0;
                }
                else if (4.096e-6 < currentAbsMax && currentAbsMax <= 40.96e-6)
                {
                    miType = MIType.IR1;
                }
                else if (40.96e-6 < currentAbsMax && currentAbsMax <= 409.6e-6)
                {
                    miType = MIType.IR2;
                }
                else if (409.6e-6 < currentAbsMax && currentAbsMax <= 4.096e-3)
                {
                    miType = MIType.IR3;
                }
                else if (4.096e-3 < currentAbsMax && currentAbsMax <= 40.96e-3)
                {
                    miType = MIType.IR4;
                }

                var imax = GetImaxFromType(miType);
                var vforceValue = GetVforceUshortValue(vforce);
                var vforceBytes = BitConverter.GetBytes(vforceValue).Reverse();
                var iclByte = GetIclOrIchValue(icl, imax);
                var ichByte = GetIclOrIchValue(ich, imax);

                //  组装数据包
                var commandList = new List<CommandInfoModel>();
                foreach (var pin in PinList)
                {
                    foreach (var site in pin.Sites)
                    {
                        var channelNum = ChannelManagerHelper.GetChannelNumBySlot(site.SiteValue);
                        if (channelNum > 0)
                        {
                            var byteList = new List<byte>();
                            byteList.Add((byte)channelNum);       //暂时按顺序下发通道（后续需要映射站点信息）
                            byteList.Add((byte)miType);      //MI
                            byteList.AddRange(vforceBytes);  //iforce
                            byteList.Add(iclByte);           //icl
                            byteList.Add(ichByte);           //ich

                            var command = new CommandInfoModel()
                            {
                                CommandCode = "0x0102",
                                CommandContent = byteList.ToArray(),
                            };
                            commandList.Add(command);
                        }
                    }
                }

                if (commandList.Any())
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, BoradType.PE, InstructionType.Configuration, commandList);
                    if (ControlService != null && message.Any())
                        ControlService.Send(PE131, message);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private double GetImaxFromType(MIType type)
        {
            double imax = 0;
            switch (type)
            {
                case MIType.IR0:
                    imax = 4.096e-6;
                    break;
                case MIType.IR1:
                    imax = 40.96e-6;
                    break;
                case MIType.IR2:
                    imax = 409.6e-6;
                    break;
                case MIType.IR3:
                    imax = 4.096e-3;
                    break;
                case MIType.IR4:
                    imax = 40.96e-3;
                    break;
            }
            return imax;
        }

        private ushort GetVforceUshortValue(double vforce)
        {
            var tempValue = (vforce + 2.56) / 132 * 1000000;
            return System.Convert.ToUInt16(Math.Floor(tempValue));
        }

        private byte GetIclOrIchValue(double i, double imax)
        {
            var tempValue = imax == 0 ? 0 : (i / imax * 1.28 + 1.75 + 2.56) * 1000 / 33.8;
            return System.Convert.ToByte(Math.Floor(tempValue));
        }

        public List<ChannelResultModel<DriverResultModel>> GetDriverAndComparator()
        {
            var result = new List<ChannelResultModel<DriverResultModel>>();
            if (PinList == null || !PinList.Any())
                return result;

            try
            {
                //  组装数据包
                var commandList = new List<CommandInfoModel>();
                foreach (var pin in PinList)
                {
                    foreach (var site in pin.Sites)
                    {
                        var channelNum = ChannelManagerHelper.GetChannelNumBySlot(site.SiteValue);
                        if (channelNum > 0)
                        {
                            var byteList = new List<byte>();
                            byteList.Add((byte)channelNum);    //暂时按顺序下发通道（后续需要映射站点信息）

                            var command = new CommandInfoModel()
                            {
                                CommandCode = "0x0100",
                                CommandContent = byteList.ToArray(),
                            };
                            commandList.Add(command);
                        }
                    }
                }

                if (commandList.Any())
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, BoradType.PE, InstructionType.Query, commandList);
                    if (ControlService != null && message.Any())
                    {
                        var queryResult = ControlService.Query(PE131, message);
                    }
                }

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<ChannelResultModel<double>> GetVoltageForce()
        {
            var result = new List<ChannelResultModel<double>>();

            if (PinList == null || !PinList.Any())
                return result;

            try
            {
                //  组装数据包
                var commandList = new List<CommandInfoModel>();
                foreach (var pin in PinList)
                {
                    foreach (var site in pin.Sites)
                    {
                        var channelNum = ChannelManagerHelper.GetChannelNumBySlot(site.SiteValue);
                        if (channelNum > 0)
                        {
                            var byteList = new List<byte>();
                            byteList.Add((byte)channelNum);       //暂时按顺序下发通道（后续需要映射站点信息）

                            var command = new CommandInfoModel()
                            {
                                CommandCode = "0x0102",
                                CommandContent = byteList.ToArray(),
                            };
                            commandList.Add(command);
                        }
                    }
                }

                if (commandList.Any())
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, BoradType.PE, InstructionType.Query, commandList);
                    if (ControlService != null && message.Any())
                    {
                        var queryResult = ControlService.Query(PE131, message);

                    }
                }

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<ChannelResultModel<double>> GetCurrentForce()
        {
            var result = new List<ChannelResultModel<double>>();

            if (PinList == null || !PinList.Any())
                return result;

            try
            {
                //  组装数据包
                var commandList = new List<CommandInfoModel>();
                foreach (var pin in PinList)
                {
                    foreach (var site in pin.Sites)
                    {
                        var channelNum = ChannelManagerHelper.GetChannelNumBySlot(site.SiteValue);
                        if (channelNum > 0)
                        {
                            var byteList = new List<byte>();
                            byteList.Add((byte)channelNum);       //暂时按顺序下发通道（后续需要映射站点信息）

                            var command = new CommandInfoModel()
                            {
                                CommandCode = "0x0101",
                                CommandContent = byteList.ToArray(),
                            };
                            commandList.Add(command);
                        }
                    }
                }

                if (commandList.Any())
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, BoradType.PE, InstructionType.Query, commandList);
                    if (ControlService != null && message.Any())
                    {
                        var queryResult = ControlService.Query(PE131, message);

                    }
                }

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
