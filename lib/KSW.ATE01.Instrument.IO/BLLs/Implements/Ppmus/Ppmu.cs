using KSW.ATE01.Instrument.IO.BLLs.Abstractions.Ppmus;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Instruments;
using KSW.ATE01.Instrument.IO.Enums.Instruments;
using KSW.ATE01.Instrument.IO.Enums.Ppmus;
using KSW.ATE01.Instrument.IO.Helpers;
using KSW.ATE01.Instrument.IO.Models.Instruments;
using KSW.ATE01.Instrument.IO.Models.Results;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Models.Errors;
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
                ErrorMessages.DpsPpmu.PinListIsNullOrEmpty();

            Instance.GetPinList(pinList);

            return Instance;
        }

        public void SetDriverAndComparator(double vil, double vih, double vol, double voh, double vt, double iol, double ioh, bool activeLoad, HizType hiz, byte dpc, byte diff = 0)
        {
            if (PinList == null || !PinList.Any())
                return;

            try
            {
                var controlService = InstrumentManagerHelper.GetControlServiceByBoardType(BoardType);

                if (vil < -2.56 || vil > 6.09)
                    ErrorMessages.DpsPpmu.DriverAndComparatorOutOfRange(nameof(vil), new object[] { vil, "-2.56V", "6.09V" });

                if (vih < -2.56 || vih > 6.09)
                    ErrorMessages.DpsPpmu.DriverAndComparatorOutOfRange(nameof(vih), new object[] { vih, "-2.56V", "6.09V" });

                if (vol < -2.56 || vol > 6.09)
                    ErrorMessages.DpsPpmu.DriverAndComparatorOutOfRange(nameof(vol), new object[] { vol, "-2.56V", "6.09V" });

                if (voh < -2.56 || voh > 6.09)
                    ErrorMessages.DpsPpmu.DriverAndComparatorOutOfRange(nameof(voh), new object[] { voh, "-2.56V", "6.09V" });

                if (vt < -2.56 || vt > 6.09)
                    ErrorMessages.DpsPpmu.DriverAndComparatorOutOfRange(nameof(vt), new object[] { vt, "-2.56V", "6.09V" });

                if (iol < 0 || iol > 25.5)
                    ErrorMessages.DpsPpmu.DriverAndComparatorOutOfRange(nameof(iol), new object[] { iol, "0mA", "25.5mA" });

                if (ioh < 0 || ioh > 25.5)
                    ErrorMessages.DpsPpmu.DriverAndComparatorOutOfRange(nameof(ioh), new object[] { ioh, "0mA", "25.5mA" });

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
                var commandListDic = new Dictionary<int, List<CommandInfoModel>>();
                foreach (var pin in PinList)
                {
                    foreach (var site in pin.Sites)
                    {
                        if (!ChannelManagerHelper.IsSiteValid(site.SiteName))
                            continue;

                        var channelNum = ChannelManagerHelper.GetChannelNumSiteInfo(site.SiteValue, out int slot);

                        var commandList = new List<CommandInfoModel>();
                        if (!commandListDic.ContainsKey(slot))
                            commandListDic[slot] = commandList;
                        else
                            commandList = commandListDic[slot];

                        if (channelNum >= 0)
                        {
                            var byteList = new List<byte>();
                            byteList.Add((byte)channelNum);
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
                            byteList.Add(diff);  //DPC

                            var command = new CommandInfoModel()
                            {
                                CommandCode = "0x0100",
                                CommandContent = byteList.ToArray(),
                            };
                            commandList.Add(command);
                        }
                    }
                }

                if (commandListDic.Any())
                {
                    foreach (var commandList in commandListDic)
                    {
                        var slotNum = $"0x{commandList.Key.ToString("x2")}";
                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType.PE, InstructionType.Configuration, commandList.Value);

                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(BoardType, slotNum);

                        if (controlService != null && instrumentInfo != null)
                            controlService.Send(instrumentInfo, message);
                    }
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
                    ErrorMessages.DpsPpmu.FIMVOutOfRange(nameof(vcl), new object[] { vcl, "-2.56V", "6.06V" });

                if (vch < -2.56 || vch > 6.06)
                    ErrorMessages.DpsPpmu.FIMVOutOfRange(nameof(vch), new object[] { vch, "-2.56V", "6.06V" });

                var imax = GetImaxFromType(iMType);
                var iforceValue = GetIforceUshortValue(iforce, imax);
                var iforceBytes = BitConverter.GetBytes(iforceValue).Reverse();
                var vclByte = GetVclOrVchValue(vcl);
                var vchByte = GetVclOrVchValue(vch);

                //  组装数据包
                var commandListDic = new Dictionary<int, List<CommandInfoModel>>();
                var controlService = InstrumentManagerHelper.GetControlServiceByBoardType(BoardType);

                foreach (var pin in PinList)
                {
                    foreach (var site in pin.Sites)
                    {
                        if (!ChannelManagerHelper.IsSiteValid(site.SiteName))
                            continue;

                        var channelNum = ChannelManagerHelper.GetChannelNumSiteInfo(site.SiteValue, out int slot);

                        var commandList = new List<CommandInfoModel>();
                        if (!commandListDic.ContainsKey(slot))
                            commandListDic[slot] = commandList;
                        else
                            commandList = commandListDic[slot];

                        if (channelNum >= 0)
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

                if (commandListDic.Any())
                {
                    foreach (var commandList in commandListDic)
                    {
                        var slotNum = $"0x{commandList.Key.ToString("x2")}";
                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType.PE, InstructionType.Configuration, commandList.Value);
                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(BoardType, slotNum);

                        if (controlService != null && instrumentInfo != null)
                            controlService.Send(instrumentInfo, message);
                    }
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
                    ErrorMessages.DpsPpmu.FVMIOutOfRange(nameof(vforce), new object[] { vforce, "-1V", "5V" });

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
                var commandListDic = new Dictionary<int, List<CommandInfoModel>>();
                var controlService = InstrumentManagerHelper.GetControlServiceByBoardType(BoardType);

                foreach (var pin in PinList)
                {
                    foreach (var site in pin.Sites)
                    {
                        if (!ChannelManagerHelper.IsSiteValid(site.SiteName))
                            continue;

                        var channelNum = ChannelManagerHelper.GetChannelNumSiteInfo(site.SiteValue, out int slot);

                        var commandList = new List<CommandInfoModel>();
                        if (!commandListDic.ContainsKey(slot))
                            commandListDic[slot] = commandList;
                        else
                            commandList = commandListDic[slot];

                        if (channelNum >= 0)
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

                if (commandListDic.Any())
                {
                    foreach (var commandList in commandListDic)
                    {
                        var slotNum = $"0x{commandList.Key.ToString("x2")}";
                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType.PE, InstructionType.Configuration, commandList.Value);

                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(BoardType, slotNum);

                        if (controlService != null && instrumentInfo != null)
                            controlService.Send(instrumentInfo, message);
                    }
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
                var commandListDic = new Dictionary<int, List<CommandInfoModel>>();
                var controlService = InstrumentManagerHelper.GetControlServiceByBoardType(BoardType);

                foreach (var pin in PinList)
                {
                    foreach (var site in pin.Sites)
                    {
                        if (!ChannelManagerHelper.IsSiteValid(site.SiteName))
                            continue;

                        var channelNum = ChannelManagerHelper.GetChannelNumSiteInfo(site.SiteValue, out int slot);

                        var commandList = new List<CommandInfoModel>();
                        if (!commandListDic.ContainsKey(slot))
                            commandListDic[slot] = commandList;
                        else
                            commandList = commandListDic[slot];

                        if (channelNum >= 0)
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

                if (commandListDic.Any())
                {
                    foreach (var commandList in commandListDic)
                    {
                        var slotNum = $"0x{commandList.Key.ToString("x2")}";
                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType.PE, InstructionType.Query, commandList.Value);

                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(BoardType, slotNum);

                        if (controlService != null && message.Any())
                        {
                            var queryResult = controlService.Query(instrumentInfo, message);
                            var commands = CommandHelper.ConversionBytesToCommands(queryResult);

                            foreach (var command in commands)
                            {
                                var tempData = ContentToDriverAndComparator(command.CommandContent);
                                if (tempData != null)
                                {
                                    result.Add(tempData);
                                }
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private ChannelResultModel<DriverResultModel> ContentToDriverAndComparator(byte[] commandContent)
        {
            ChannelResultModel<DriverResultModel> result = null;

            try
            {
                if (commandContent.Any())
                {
                    result = new ChannelResultModel<DriverResultModel>();
                    var index = 0;
                    result.ChannelNum = commandContent[index++];
                    result.OriginalData = commandContent;
                    result.Site = ChannelManagerHelper.GetSlotByChannelNum(result.ChannelNum);
                    result.PinName = PinManagerHelper.GetPinNameBySlotName(TestPlan?.Channel, result.Site);
                    result.SiteResult = new DriverResultModel();
                    var vilBytes = commandContent.AsSpan().Slice(index, 2).ToArray().Reverse().ToArray();
                    var vilShort = BitConverter.ToUInt16(vilBytes);
                    index += 2;
                    result.SiteResult.Vil = GetDoubleByUshort(vilShort);

                    var vihBytes = commandContent.AsSpan().Slice(index, 2).ToArray().Reverse().ToArray();
                    var vihShort = BitConverter.ToUInt16(vihBytes);
                    index += 2;
                    result.SiteResult.Vih = GetDoubleByUshort(vihShort);

                    var volBytes = commandContent.AsSpan().Slice(index, 2).ToArray().Reverse().ToArray();
                    var volShort = BitConverter.ToUInt16(volBytes);
                    index += 2;
                    result.SiteResult.Vol = GetDoubleByUshort(volShort);

                    var vohBytes = commandContent.AsSpan().Slice(index, 2).ToArray().Reverse().ToArray();
                    var vohShort = BitConverter.ToUInt16(vohBytes);
                    index += 2;
                    result.SiteResult.Voh = GetDoubleByUshort(vohShort);

                    var vtBytes = commandContent.AsSpan().Slice(index, 2).ToArray().Reverse().ToArray();
                    var vtShort = BitConverter.ToUInt16(vtBytes);
                    index += 2;
                    result.SiteResult.Vt = GetDoubleByUshort(vtShort);

                    result.SiteResult.Iol = GetDoubleByByte(commandContent[index++]);
                    result.SiteResult.Ioh = GetDoubleByByte(commandContent[index++]);
                    result.SiteResult.ActiveLoad = System.Convert.ToBoolean(commandContent[index++]);

                    return result;
                }
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        private double GetDoubleByUshort(ushort original)
        {
            return original * 1.0 / 1000000 * 132 - 2.56;
        }

        private double GetDoubleByByte(byte v)
        {
            return v * 0.1;
        }

        public List<ChannelResultModel<double>> GetVoltageForce()
        {
            var result = new List<ChannelResultModel<double>>();

            if (PinList == null || !PinList.Any())
                return result;

            try
            {
                //  组装数据包
                var commandListDic = new Dictionary<int, List<CommandInfoModel>>();
                var controlService = InstrumentManagerHelper.GetControlServiceByBoardType(BoardType);

                foreach (var pin in PinList)
                {
                    foreach (var site in pin.Sites)
                    {
                        if (!ChannelManagerHelper.IsSiteValid(site.SiteName))
                            continue;

                        var channelNum = ChannelManagerHelper.GetChannelNumSiteInfo(site.SiteValue, out int slot);

                        var commandList = new List<CommandInfoModel>();
                        if (!commandListDic.ContainsKey(slot))
                            commandListDic[slot] = commandList;
                        else
                            commandList = commandListDic[slot];

                        if (channelNum >= 0)
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

                if (commandListDic.Any())
                {
                    foreach (var commandList in commandListDic)
                    {
                        var slotNum = $"0x{commandList.Key.ToString("x2")}";
                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType.PE, InstructionType.Query, commandList.Value);

                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(BoardType, slotNum);

                        if (controlService != null && message.Any())
                        {
                            var queryResult = controlService.Query(instrumentInfo, message);
                            var commands = CommandHelper.ConversionBytesToCommands(queryResult);

                            foreach (var command in commands)
                            {
                                var tempData = ContentToVoltageForce(command.CommandContent);
                                if (tempData != null)
                                {
                                    result.Add(tempData);
                                }
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private ChannelResultModel<double> ContentToVoltageForce(byte[] commandContent)
        {
            ChannelResultModel<double> result = null;

            try
            {
                if (commandContent.Any())
                {
                    result = new ChannelResultModel<double>();
                    var index = 0;
                    result.ChannelNum = commandContent[index++];
                    result.OriginalData = commandContent;
                    result.Site = ChannelManagerHelper.GetSlotByChannelNum(result.ChannelNum);
                    result.PinName = PinManagerHelper.GetPinNameBySlotName(TestPlan?.Channel, result.Site);
                    var imType = (MIType)commandContent[index++];
                    var vforceBytes = commandContent.AsSpan().Slice(index, 2).ToArray().Reverse().ToArray();
                    var vforceShort = BitConverter.ToUInt16(vforceBytes);
                    var vforce = GetVforceByUshort(vforceShort);
                    result.SiteResult = vforce;

                    return result;
                }
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        private double GetVforceByUshort(ushort v)
        {
            return v * 1.0 / 1000000 * 132 - 2.56;
        }

        public List<ChannelResultModel<double>> GetCurrentForce()
        {
            var result = new List<ChannelResultModel<double>>();

            if (PinList == null || !PinList.Any())
                return result;

            try
            {
                //  组装数据包
                var commandListDic = new Dictionary<int, List<CommandInfoModel>>();
                var controlService = InstrumentManagerHelper.GetControlServiceByBoardType(BoardType);

                foreach (var pin in PinList)
                {
                    foreach (var site in pin.Sites)
                    {
                        if (!ChannelManagerHelper.IsSiteValid(site.SiteName))
                            continue;

                        var channelNum = ChannelManagerHelper.GetChannelNumSiteInfo(site.SiteValue, out int slot);

                        var commandList = new List<CommandInfoModel>();
                        if (!commandListDic.ContainsKey(slot))
                            commandListDic[slot] = commandList;
                        else
                            commandList = commandListDic[slot];

                        if (channelNum >= 0)
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

                if (commandListDic.Any())
                {
                    foreach (var commandList in commandListDic)
                    {
                        var slotNum = $"0x{commandList.Key.ToString("x2")}";

                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType.PE, InstructionType.Query, commandList.Value);
                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(BoardType, slotNum);

                        if (controlService != null && message.Any())
                        {
                            var queryResult = controlService.Query(instrumentInfo, message);
                            var commands = CommandHelper.ConversionBytesToCommands(queryResult);

                            foreach (var command in commands)
                            {
                                var tempData = ContentToCurrentForce(command.CommandContent);
                                if (tempData != null)
                                {
                                    result.Add(tempData);
                                }
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private ChannelResultModel<double> ContentToCurrentForce(byte[] commandContent)
        {
            ChannelResultModel<double> result = null;

            try
            {
                if (commandContent.Any())
                {
                    result = new ChannelResultModel<double>();
                    var index = 0;
                    result.ChannelNum = commandContent[index++];
                    result.OriginalData = commandContent;
                    result.Site = ChannelManagerHelper.GetSlotByChannelNum(result.ChannelNum);
                    result.PinName = PinManagerHelper.GetPinNameBySlotName(TestPlan?.Channel, result.Site);
                    var imType = (IMType)commandContent[index++];
                    var iforceBytes = commandContent.AsSpan().Slice(index, 2).ToArray().Reverse().ToArray();
                    var iforceShort = BitConverter.ToUInt16(iforceBytes);
                    var imax = GetImaxFromType(imType);
                    var iforce = GetIforceByUshort(iforceShort, imax);
                    result.SiteResult = iforce;

                    return result;
                }
            }
            catch (Exception e)
            {

                throw e;
            }

            return result;
        }

        private double GetIforceByUshort(ushort i, double imax)
        {
            return ((i * 1.0) / 1000000 * 132 - 1.75 - 2.56) / 1.28 * imax;
        }
    }
}
