using KSW.ATE01.Instrument.IO.BLLs.Abstractions.Commons;
using KSW.ATE01.Instrument.IO.BLLs.Abstractions.Dps;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Instruments;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Patterns;
using KSW.ATE01.Instrument.IO.Enums.Dps;
using KSW.ATE01.Instrument.IO.Enums.Instruments;
using KSW.ATE01.Instrument.IO.Enums.Patterns;
using KSW.ATE01.Instrument.IO.Enums.Ppmus;
using KSW.ATE01.Instrument.IO.Helpers;
using KSW.ATE01.Instrument.IO.Models.Instruments;
using KSW.ATE01.Instrument.IO.Models.Results;
using KSW.ATE01.Project.Base.Enums.TestPlans;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Models;
using KSW.ATE01.Project.Base.Models.Errors;
using KSW.ATE01.Project.Base.Models.Patterns;
using KSW.ATE01.Project.Base.Models.TestPlans;
using System.IO;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.Dps
{
    public class Dps : DpsCommandBase<Dps>, IDps, ICommon
    {
        private const double _periodResolution = 6.25e-10;
        private const double _ns = 1e-9;
        private const long _mbByte = 128 * 1024 * 1024L;
        private const int _maxChannelNum = 127;
        private const int _packageAdditionalLength = 8;
        private double _iforce;
        private double _vcl;
        private double _vch;
        private IRType _irType;
        private List<PatternModel> _patterns = new List<PatternModel>();

        public static IDps Pins(string pinList)
        {
            if (string.IsNullOrEmpty(pinList))
                ErrorMessages.DpsPpmu.PinListIsNullOrEmpty();

            Instance.GetPinList(pinList);

            return Instance;
        }

        public static ICommon Common(string pinList)
        {
            if (string.IsNullOrEmpty(pinList))
                ErrorMessages.DpsPpmu.PinListIsNullOrEmpty();

            Instance.CommonPinList = pinList;

            return Instance;
        }

        public void SetDriverAndComparator(double vil, double vih, double vol, double voh, double vt, HizType hiz, bool isDriver50Ω)
        {
            if (PinList == null || !PinList.Any())
                return;

            var commandListDic = new Dictionary<int, List<CommandInfoModel>>();

            try
            {
                if (vil < -4 || vil > 10)
                    ErrorMessages.DpsPpmu.DriverAndComparatorOutOfRange(nameof(vil), new object[] { vil, "-4V", "10V" });

                if (vih < -4 || vih > 10)
                    ErrorMessages.DpsPpmu.DriverAndComparatorOutOfRange(nameof(vih), new object[] { vih, "-4V", "10V" });

                if (vol < -4 || vol > 10)
                    ErrorMessages.DpsPpmu.DriverAndComparatorOutOfRange(nameof(vol), new object[] { vol, "-4V", "10V" });

                if (voh < -4 || voh > 10)
                    ErrorMessages.DpsPpmu.DriverAndComparatorOutOfRange(nameof(voh), new object[] { voh, "-4V", "10V" });

                if (vt < -4 || vt > 10)
                    ErrorMessages.DpsPpmu.DriverAndComparatorOutOfRange(nameof(vt), new object[] { vt, "-4V", "10V" });

                // 转换数据格式
                var vilValue = GetUshortValue(vil);
                var vilBytes = BitConverter.GetBytes(vilValue).Reverse();
                var vihValue = GetUshortValue(vih);
                var vihBytes = BitConverter.GetBytes(vihValue).Reverse();
                var volValue = GetUshortValue(vol);
                var volBytes = BitConverter.GetBytes(volValue).Reverse();
                var vohValue = GetUshortValue(voh);
                var vohBytes = BitConverter.GetBytes(vohValue).Reverse();
                var vtValue = GetUshortValue(voh);
                var vtBytes = BitConverter.GetBytes(vtValue).Reverse();

                foreach (var pin in PinList)
                {
                    foreach (var site in pin.Sites)
                    {
                        if (!ChannelManagerHelper.IsSiteValid(site.SiteName))
                            continue;

                        var channelNum = ChannelManagerHelper.GetChannelNumSiteInfo(site.SiteValue, out int slot);

                        List<CommandInfoModel> commandList = new List<CommandInfoModel>();
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
                            byteList.Add((byte)hiz);  //Hiz模式，0:hiz，1:vt
                            byteList.Add(System.Convert.ToByte(isDriver50Ω));

                            var command = new CommandInfoModel()
                            {
                                CommandCode = "0x0110",
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
                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType.DPS, InstructionType.Configuration, commandList.Value);

                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(BoardType, slotNum);

                        if (ControlService != null && instrumentInfo != null)
                            ControlService.Send(instrumentInfo, message);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private ushort GetUshortValue(double v)
        {
            var tempValue = (v + 4) / 14.0 * 65535;
            return System.Convert.ToUInt16(Math.Floor(tempValue));
        }

        public void SetTimingDetail(double period, Timingformat fmt, StrobeModeType strobeMode, double d0, double d1, double d2, double d3, double r0, double r1)
        {
            if (PinList == null || !PinList.Any())
                return;

            var periodDouble = period * _ns;

            if (periodDouble < 0 || periodDouble > 0.02684354559375)
                throw new ArgumentOutOfRangeException("Period", "取值范围:0~0.02684354559375");

            var periodUInt = System.Convert.ToUInt32(period / _periodResolution);
            var periodBytes = BitConverter.GetBytes(periodUInt).Reverse();

            double driveADouble = d0 * _ns;
            var driveAUInt = System.Convert.ToUInt32(driveADouble / _periodResolution);
            var driveABytes = BitConverter.GetBytes(driveAUInt).Reverse();

            double driveBDouble = d1 * _ns;
            var driveBUInt = System.Convert.ToUInt32(driveBDouble / _periodResolution);
            var driveBBytes = BitConverter.GetBytes(driveBUInt).Reverse();

            double driveCDouble = d2 * _ns;
            var driveCUInt = System.Convert.ToUInt32(driveCDouble / _periodResolution);
            var driveCBytes = BitConverter.GetBytes(driveCUInt).Reverse();

            double driveDDouble = d3 * _ns;
            var driveDUInt = System.Convert.ToUInt32(driveDDouble / _periodResolution);
            var driveDBytes = BitConverter.GetBytes(driveDUInt).Reverse();

            var formatByte = System.Convert.ToByte(fmt);
            var strobeByte = System.Convert.ToByte(strobeMode);

            double r0Period = r0 * _ns;
            var r0PeriodUInt = System.Convert.ToUInt32(r0Period / _periodResolution);
            var r0PeriodBytes = BitConverter.GetBytes(r0PeriodUInt).Reverse();

            double r1Period = r1 * _ns;
            var r1PeriodUInt = System.Convert.ToUInt32(r1Period / _periodResolution);
            var r1PeriodBytes = BitConverter.GetBytes(r1PeriodUInt).Reverse();

            #region 校准数据
            byte d_d_d = 0;
            byte en_d_d = 0;
            short den_d_c = 0;
            byte ca_d_d = 0;
            byte cb_d_d = 0;
            short cab_d_c = 0;
            #endregion

            var den_d_cBytes = BitConverter.GetBytes(den_d_c).Reverse();
            var cab_d_cBytes = BitConverter.GetBytes(cab_d_c).Reverse();
            var commandListDic = new Dictionary<int, List<CommandInfoModel>>();

            try
            {
                var controlService = Instance.ControlService;
                foreach (var pin in PinList)
                {
                    foreach (var site in pin.Sites)
                    {
                        if (!ChannelManagerHelper.IsSiteValid(site.SiteName))
                            continue;

                        var channelNum = ChannelManagerHelper.GetChannelNumSiteInfo(site.SiteValue, out int slot);
                        List<CommandInfoModel> commandList = new List<CommandInfoModel>();
                        if (!commandListDic.ContainsKey(slot))
                            commandListDic[slot] = commandList;
                        else
                            commandList = commandListDic[slot];
                        if (channelNum >= 0)
                        {
                            var byteList = new List<byte>();
                            byteList.Add((byte)channelNum);
                            byteList.AddRange(periodBytes);
                            byteList.Add(formatByte);
                            byteList.Add(strobeByte);
                            byteList.AddRange(driveABytes);   //D0
                            byteList.AddRange(driveBBytes);   //D1
                            byteList.AddRange(driveCBytes);   //D2
                            byteList.AddRange(driveDBytes);   //D3
                            byteList.AddRange(r0PeriodBytes); //R0
                            byteList.AddRange(r1PeriodBytes); //R1
                            byteList.Add(d_d_d);              //d_d_d
                            byteList.Add(en_d_d);              //en_d_d
                            byteList.AddRange(den_d_cBytes);
                            byteList.Add(ca_d_d);              //ca_d_d
                            byteList.Add(cb_d_d);              //cb_d_d
                            byteList.AddRange(cab_d_cBytes);
                            var command = new CommandInfoModel()
                            {
                                CommandCode = "0x0122",
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
                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType.DPS, InstructionType.Configuration, commandList.Value);

                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType((BoardType)Instance?.BoardType, slotNum);

                        if (controlService != null && instrumentInfo != null)
                            controlService.Send(instrumentInfo, message);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void SetTimingByPins(byte d_d_d = 0, byte en_d_d = 0, short den_d_c = 0, byte ca_d_d = 0, byte cb_d_d = 0, short cab_d_c = 0)
        {
            if (PinList == null || !PinList.Any())
                return;

            if (d_d_d < 0 || d_d_d > 31)
                throw new ArgumentOutOfRangeException(nameof(d_d_d), "取值范围:0~ 31");

            if (en_d_d < 0 || en_d_d > 31)
                throw new ArgumentOutOfRangeException(nameof(en_d_d), "取值范围:0~ 31");

            if (den_d_c < 0 || den_d_c > 1023)
                throw new ArgumentOutOfRangeException(nameof(den_d_c), "取值范围:0~ 1023");

            if (ca_d_d < 0 || ca_d_d > 31)
                throw new ArgumentOutOfRangeException(nameof(ca_d_d), "取值范围:0~ 31");

            if (cb_d_d < 0 || cb_d_d > 31)
                throw new ArgumentOutOfRangeException(nameof(cb_d_d), "取值范围:0~ 31");

            if (cab_d_c < 0 || cab_d_c > 31)
                throw new ArgumentOutOfRangeException(nameof(cab_d_c), "取值范围:0~ 1023");

            var commonData = CommonData.Instance;
            if (commonData == null || commonData.TestPlan == null || commonData.TestPlan?.Channel == null || commonData.TestPlan?.TestItem == null)
                return;

            if (!string.IsNullOrEmpty(commonData.Timing))
            {
                SetTimingDetail(d_d_d, en_d_d, den_d_c, ca_d_d, cb_d_d, cab_d_c);
            }
        }

        private void SetTimingDetail(byte d_d_d, byte en_d_d, short den_d_c, byte ca_d_d, byte cb_d_d, short cab_d_c, List<ChannelModel> pinList = null)
        {
            var commonData = CommonData.Instance;
            var controlService = Instance.ControlService;

            var testItem = commonData.TestPlan?.TestItem?.FirstOrDefault(x => x.TestItemName.Equals(commonData.TestItemName));
            var args = commonData.TestItemArgs;

            if (pinList == null || !pinList.Any())
                pinList = commonData.TestPlan.Channel;
            if (args.Any())
            {
                var patternFileName = args.FirstOrDefault()?.ParamValue;
                //var patterns = Pattern.Instance.Patterns;
                var patterns = Pattern.Instance.BinPatterns;
                var patternFile = patterns.FirstOrDefault(x => x.FileName.Equals(patternFileName));
                var timings = testItem?.Timings;
                if (patternFile != null)
                {
                    var timingSets = patternFile.PinPacks.Where(x => !string.IsNullOrEmpty(x.TimingSet)).Select(x => x.TimingSet);
                    var patternTimings = timings?.Where(x => timingSets.Contains(x.TimingName));

                    if (patternTimings.Any())
                    {
                        foreach (var patternTiming in patternTimings)
                        {
                            var currentPinList = pinList.Where(x => x.Groups.Any(y => y.Name.ToLower().Equals(patternTiming.PinName.ToLower()))).ToList();
                            if (currentPinList == null || !currentPinList.Any())
                                currentPinList = pinList.Where(x => x.PinName.ToLower().Equals(patternTiming.PinName.ToLower())).ToList();

                            var period = System.Convert.ToDouble(patternTiming.Period) * _ns;

                            if (period < 0 || period > 0.02684354559375)
                                throw new ArgumentOutOfRangeException("Period", "取值范围:0~0.02684354559375");

                            var periodUInt = System.Convert.ToUInt32(period / _periodResolution);
                            var periodBytes = BitConverter.GetBytes(periodUInt).Reverse();

                            double driveA = 0;
                            double.TryParse(patternTiming.DriveA, out driveA);
                            driveA *= _ns;
                            var driveAUInt = System.Convert.ToUInt32(driveA / _periodResolution);
                            var driveABytes = BitConverter.GetBytes(driveAUInt).Reverse();

                            double driveB = 0;
                            double.TryParse(patternTiming.DriveB, out driveB);
                            driveB *= _ns;
                            var driveBUInt = System.Convert.ToUInt32(driveB / _periodResolution);
                            var driveBBytes = BitConverter.GetBytes(driveBUInt).Reverse();

                            double driveC = 0;
                            double.TryParse(patternTiming.DriveC, out driveC);
                            driveC *= _ns;
                            var driveCUInt = System.Convert.ToUInt32(driveC / _periodResolution);
                            var driveCBytes = BitConverter.GetBytes(driveCUInt).Reverse();

                            double driveD = 0;
                            double.TryParse(patternTiming.DriveD, out driveD);
                            driveD *= _ns;
                            var driveDUInt = System.Convert.ToUInt32(driveD / _periodResolution);
                            var driveDBytes = BitConverter.GetBytes(driveDUInt).Reverse();

                            var formatByte = System.Convert.ToByte(patternTiming.Fmt);
                            var strobeByte = System.Convert.ToByte(patternTiming.StrobeMode);

                            double r0Period = System.Convert.ToDouble(patternTiming.StrobeA) * _ns;
                            var r0PeriodUInt = System.Convert.ToUInt32(r0Period / _periodResolution);
                            var r0PeriodBytes = BitConverter.GetBytes(r0PeriodUInt).Reverse();

                            double r1Period = System.Convert.ToDouble(patternTiming.StrobeB) * _ns;
                            var r1PeriodUInt = System.Convert.ToUInt32(r1Period / _periodResolution);
                            var r1PeriodBytes = BitConverter.GetBytes(r1PeriodUInt).Reverse();

                            var den_d_cBytes = BitConverter.GetBytes(den_d_c).Reverse();
                            var cab_d_cBytes = BitConverter.GetBytes(cab_d_c).Reverse();

                            var commandListDic = new Dictionary<int, List<CommandInfoModel>>();

                            foreach (var pin in currentPinList)
                            {
                                foreach (var site in pin.Sites)
                                {
                                    if (!ChannelManagerHelper.IsSiteValid(site.SiteName))
                                        continue;

                                    var channelNum = ChannelManagerHelper.GetChannelNumSiteInfo(site.SiteValue, out int slot);


                                    List<CommandInfoModel> commandList = new List<CommandInfoModel>();
                                    if (!commandListDic.ContainsKey(slot))
                                        commandListDic[slot] = commandList;
                                    else
                                        commandList = commandListDic[slot];
                                    if (channelNum >= 0)
                                    {
                                        var byteList = new List<byte>();
                                        byteList.Add((byte)channelNum);
                                        byteList.AddRange(periodBytes);
                                        byteList.Add(formatByte);
                                        byteList.Add(strobeByte);
                                        byteList.AddRange(driveABytes);   //D0
                                        byteList.AddRange(driveBBytes);   //D1
                                        byteList.AddRange(driveCBytes);   //D2
                                        byteList.AddRange(driveDBytes);   //D3
                                        byteList.AddRange(r0PeriodBytes); //R0
                                        byteList.AddRange(r1PeriodBytes); //R1
                                        byteList.Add(d_d_d);              //d_d_d
                                        byteList.Add(en_d_d);              //en_d_d
                                        byteList.AddRange(den_d_cBytes);
                                        byteList.Add(ca_d_d);              //ca_d_d
                                        byteList.Add(cb_d_d);              //cb_d_d
                                        byteList.AddRange(cab_d_cBytes);
                                        var command = new CommandInfoModel()
                                        {
                                            CommandCode = "0x0122",
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
                                    var message = CommandHelper.GetCommandBytes(0xFF, BoardType.DPS, InstructionType.Configuration, commandList.Value);

                                    var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType((BoardType)Instance?.BoardType, slotNum);

                                    if (controlService != null && instrumentInfo != null)
                                        controlService.Send(instrumentInfo, message);
                                }
                            }

                        }
                    }
                }
            }
        }

        public void SetFIMV(double iforce, bool isDpsVcc, bool isDpsVee, double vcl, double vch, GangType gang)
        {
            if (PinList == null || !PinList.Any())
                return;
            _iforce = iforce;
            _vcl = vcl;
            _vch = vch;

            var iRType = IRType.IR0;

            var currentAbs = Math.Abs(iforce);
            // 转换数据格式
            if (currentAbs <= 5e-6)
            {
                iRType = IRType.IR0;
            }
            else if (5e-6 < currentAbs && currentAbs <= 50e-6)
            {
                iRType = IRType.IR1;
            }
            else if (50e-6 < currentAbs && currentAbs <= 500e-6)
            {
                iRType = IRType.IR2;
            }
            else if (500e-6 < currentAbs && currentAbs <= 5e-3)
            {
                iRType = IRType.IR3;
            }
            else if (5e-3 < currentAbs && currentAbs <= 50e-3)
            {
                iRType = IRType.IR4;
            }
            else if (50e-3 < currentAbs && currentAbs <= 500e-3)
            {
                iRType = IRType.IR5;
            }

            _irType = iRType;

            try
            {
                var imax = GetImaxFromType(iRType);
                var iforceValue = GetIforceUshortValue(iforce, imax);
                var iforceBytes = BitConverter.GetBytes(iforceValue).Reverse();
                var vclByte = GetVclOrVchValue(vcl);
                var vchByte = GetVclOrVchValue(vch);

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
                            byteList.Add((byte)channelNum);       //暂时按顺序下发通道（后续需要映射站点信息）
                            byteList.Add((byte)iRType);      //IR
                            byteList.AddRange(iforceBytes);  //iforce
                            byteList.Add(System.Convert.ToByte(isDpsVcc));           //Vcc
                            byteList.Add(System.Convert.ToByte(isDpsVee));           //Vee
                            byteList.Add(vclByte);           //vcl
                            byteList.Add(vchByte);           //vch
                            byteList.Add((byte)gang);        //GANG
                            var command = new CommandInfoModel()
                            {
                                CommandCode = "0x0111",
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
                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType.DPS, InstructionType.Configuration, commandList.Value);
                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(BoardType, slotNum);

                        if (ControlService != null && instrumentInfo != null)
                            ControlService.Send(instrumentInfo, message);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private double GetImaxFromType(IRType type)
        {
            double imax = 0;
            switch (type)
            {
                case IRType.IR0:
                    imax = 5e-6;
                    break;
                case IRType.IR1:
                    imax = 50e-6;
                    break;
                case IRType.IR2:
                    imax = 500e-6;
                    break;
                case IRType.IR3:
                    imax = 5e-3;
                    break;
                case IRType.IR4:
                    imax = 50e-3;
                    break;
                case IRType.IR5:
                    imax = 500e-3;
                    break;
            }
            return imax;
        }

        private ushort GetIforceUshortValue(double i, double imax)
        {
            var tempValue = imax == 0 ? 0 : (i / imax * 0.625 + 2.5) / 5 * 65535;
            return System.Convert.ToUInt16(Math.Floor(tempValue));
        }

        private byte GetVclOrVchValue(double v)
        {
            var tempValue = (v + 4) / 14 * 255;
            return System.Convert.ToByte(Math.Floor(tempValue));
        }

        public void SetFVMI(double vforce, bool isDpsVcc, bool isDpsVee, double icl, double ich, GangType gang)
        {
            if (PinList == null || !PinList.Any())
                return;
            var miType = DpsIMType.Hiz;
            try
            {
                var currentAbsMax = Math.Max(Math.Abs(icl), Math.Abs(ich));
                // 转换数据格式
                if (currentAbsMax <= 5e-6)
                {
                    miType = DpsIMType.IM0;
                }
                else if (5e-6 < currentAbsMax && currentAbsMax <= 50e-6)
                {
                    miType = DpsIMType.IM1;
                }
                else if (50e-6 < currentAbsMax && currentAbsMax <= 500e-6)
                {
                    miType = DpsIMType.IM2;
                }
                else if (500e-6 < currentAbsMax && currentAbsMax <= 5e-3)
                {
                    miType = DpsIMType.IM3;
                }
                else if (5e-3 < currentAbsMax && currentAbsMax <= 50e-3)
                {
                    miType = DpsIMType.IM4;
                }
                else
                {
                    miType = DpsIMType.IM5;
                }

                var imax = GetImaxFromType(miType);
                var vforceValue = GetVforceUshortValue(vforce);
                var vforceBytes = BitConverter.GetBytes(vforceValue).Reverse();
                var iclByte = GetIclOrIchValue(icl, imax);
                var ichByte = GetIclOrIchValue(ich, imax);

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
                            byteList.Add((byte)channelNum);       //暂时按顺序下发通道（后续需要映射站点信息）
                            byteList.Add((byte)miType);      //MI
                            byteList.AddRange(vforceBytes);  //iforce
                            byteList.Add(System.Convert.ToByte(isDpsVcc));           //Vcc
                            byteList.Add(System.Convert.ToByte(isDpsVee));           //Vee
                            byteList.Add(iclByte);           //icl
                            byteList.Add(ichByte);           //ich
                            byteList.Add((byte)gang);        //GANG

                            var command = new CommandInfoModel()
                            {
                                CommandCode = "0x0112",
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
                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType.DPS, InstructionType.Configuration, commandList.Value);
                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(BoardType, slotNum);

                        if (ControlService != null && instrumentInfo != null)
                            ControlService.Send(instrumentInfo, message);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private double GetImaxFromType(DpsIMType type)
        {
            double imax = 0;
            switch (type)
            {
                case DpsIMType.IM0:
                    imax = 5e-6;
                    break;
                case DpsIMType.IM1:
                    imax = 50e-6;
                    break;
                case DpsIMType.IM2:
                    imax = 500e-6;
                    break;
                case DpsIMType.IM3:
                    imax = 5e-3;
                    break;
                case DpsIMType.IM4:
                    imax = 50e-3;
                    break;
                case DpsIMType.IM5:
                    imax = 500e-3;
                    break;
            }
            return imax;
        }

        private ushort GetVforceUshortValue(double vforce)
        {
            var tempValue = (vforce + 4) / 14 * 65535;
            return System.Convert.ToUInt16(Math.Floor(tempValue));
        }

        private byte GetIclOrIchValue(double i, double imax)
        {
            var tempValue = imax == 0 ? 0 : (i / imax * 0.625 + 2.5) * 255 / 4.98;
            return System.Convert.ToByte(Math.Floor(tempValue));
        }

        public void SetPinInit(PinInitVoltageType initType = PinInitVoltageType.high)
        {
            if (PinList == null || !PinList.Any())
                return;

            //  组装数据包
            var commandListDic = new Dictionary<int, List<CommandInfoModel>>();

            try
            {
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
                            byteList.Add((byte)initType);

                            var command = new CommandInfoModel()
                            {
                                CommandCode = "0x0121",
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
                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType.DPS, InstructionType.Configuration, commandList.Value);
                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(BoardType, slotNum);

                        if (ControlService != null && instrumentInfo != null)
                            ControlService.Send(instrumentInfo, message);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetPinType(PinIOType pinType = PinIOType.inout)
        {
            if (PinList == null || !PinList.Any())
                return;

            //  组装数据包
            var commandListDic = new Dictionary<int, List<CommandInfoModel>>();

            try
            {
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
                            byteList.Add((byte)pinType);

                            var command = new CommandInfoModel()
                            {
                                CommandCode = "0x0120",
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
                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType.DPS, InstructionType.Configuration, commandList.Value);
                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(BoardType, slotNum);

                        if (ControlService != null && instrumentInfo != null)
                            ControlService.Send(instrumentInfo, message);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<ChannelResultModel<double>> GetMV()
        {
            var result = new List<ChannelResultModel<double>>();
            if (PinList == null || !PinList.Any())
                return result;

            //  组装数据包
            var commandListDic = new Dictionary<int, List<CommandInfoModel>>();

            try
            {
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
                                CommandCode = "0x0113",
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
                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType.DPS, InstructionType.Query, commandList.Value);

                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(BoardType, slotNum);

                        if (ControlService != null && message.Any())
                        {
                            var queryResult = ControlService.Query(instrumentInfo, message);
                            var commands = CommandHelper.ConversionBytesToCommands(queryResult);

                            foreach (var command in commands)
                            {
                                var tempData = ContentToMV(command.SlotNum, command.CommandContent);
                                if (tempData != null)
                                {
                                    result.Add(tempData);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        private ChannelResultModel<double> ContentToMV(byte slot, byte[] commandContent)
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
                    result.Site = ChannelManagerHelper.GetSiteInfo(slot, result.ChannelNum);
                    result.PinName = PinManagerHelper.GetPinNameBySlotName(TestPlan?.Channel, result.Site);
                    result.SiteName = ChannelManagerHelper.GetSiteName(result.PinName, result.Site);
                    var codeBytes = commandContent.AsSpan().Slice(index, 2).ToArray().Reverse().ToArray();
                    var code = BitConverter.ToInt16(codeBytes);
                    var value = ((2.5 * code / 32768 - 1.25) * 2.0 + 15.0 / 14.0) * 14.0 / 5.0;
                    result.SiteResult = value;

                    return result;
                }

            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        public List<DpsMIResultModel<double>> GetMI()
        {
            var result = new List<DpsMIResultModel<double>>();
            if (PinList == null || !PinList.Any())
                return result;

            //  组装数据包
            var commandListDic = new Dictionary<int, List<CommandInfoModel>>();

            try
            {
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
                                CommandCode = "0x0114",
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
                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType.DPS, InstructionType.Query, commandList.Value);

                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(BoardType, slotNum);

                        if (ControlService != null && message.Any())
                        {
                            var queryResult = ControlService.Query(instrumentInfo, message);
                            var commands = CommandHelper.ConversionBytesToCommands(queryResult);

                            foreach (var command in commands)
                            {
                                var tempData = ContentToMI(command.SlotNum, command.CommandContent);
                                if (tempData != null)
                                {
                                    result.Add(tempData);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        private DpsMIResultModel<double> ContentToMI(byte slot, byte[] commandContent)
        {
            DpsMIResultModel<double> result = null;

            try
            {
                if (commandContent.Any())
                {
                    result = new DpsMIResultModel<double>();
                    var index = 0;
                    result.ChannelNum = commandContent[index++];
                    result.OriginalData = commandContent;
                    result.Site = ChannelManagerHelper.GetSiteInfo(slot, result.ChannelNum);
                    result.PinName = PinManagerHelper.GetPinNameBySlotName(TestPlan?.Channel, result.Site);
                    result.SiteName = ChannelManagerHelper.GetSiteName(result.PinName, result.Site);
                    result.IR = (IRType)commandContent[index++];
                    var codeBytes = commandContent.AsSpan().Slice(index, 2).ToArray().Reverse().ToArray();
                    var code = BitConverter.ToInt16(codeBytes);
                    var value = ((2.5 * code / 32768 - 1.25) * 2.0) / 2.5 * 4 * GetImaxFromType(result.IR);
                    result.SiteResult = value;
                    return result;
                }

            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        public List<ChannelResultModel<double>> GetMT()
        {
            var result = new List<ChannelResultModel<double>>();
            if (PinList == null || !PinList.Any())
                return result;

            //  组装数据包
            var commandListDic = new Dictionary<int, List<CommandInfoModel>>();

            try
            {
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
                                CommandCode = "0x0115",
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
                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType.DPS, InstructionType.Query, commandList.Value);

                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(BoardType, slotNum);

                        if (ControlService != null && message.Any())
                        {
                            var queryResult = ControlService.Query(instrumentInfo, message);
                            var commands = CommandHelper.ConversionBytesToCommands(queryResult);

                            foreach (var command in commands)
                            {
                                var tempData = ContentToMT(command.SlotNum, command.CommandContent);
                                if (tempData != null)
                                {
                                    result.Add(tempData);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        private ChannelResultModel<double> ContentToMT(byte slot, byte[] commandContent)
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
                    result.Site = ChannelManagerHelper.GetSiteInfo(slot, result.ChannelNum);
                    result.PinName = PinManagerHelper.GetPinNameBySlotName(TestPlan?.Channel, result.Site);
                    result.SiteName = ChannelManagerHelper.GetSiteName(result.PinName, result.Site);
                    var codeBytes = commandContent.AsSpan().Slice(index, 2).ToArray().Reverse().ToArray();
                    var code = BitConverter.ToInt16(codeBytes);
                    var value = code / 100.0;
                    result.SiteResult = value;

                    return result;
                }

            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        public static Dictionary<string, DpsBoardInfoModel> GetBoardInfo()
        {
            var result = new Dictionary<string, DpsBoardInfoModel>();
            var controlService = Instance?.ControlService;
            var detailInfos = InstrumentManagerHelper.GetDetailInfosByBoardType(BoardType.DPS);

            var command = new CommandInfoModel()
            {
                CommandCode = "0x0140"
            };

            var message = CommandHelper.GetCommandBytes(0xFF, BoardType.DPS, InstructionType.Query, new List<CommandInfoModel>() { command });

            foreach (var detailInfo in detailInfos)
            {
                var instrumentInfo = detailInfo.Value;

                if (controlService != null && instrumentInfo != null)
                {
                    var queryResult = controlService.Query(instrumentInfo, message);
                    var resultCommands = CommandHelper.ConversionBytesToCommands(queryResult);
                    var tempData = new DpsBoardInfoModel();
                    if (!result.ContainsKey(detailInfo.Key.SlotNum))
                        result.Add(detailInfo.Key.SlotNum, tempData);
                    else
                        tempData = result[detailInfo.Key.SlotNum];

                    foreach (var resultCommand in resultCommands)
                    {
                        if (resultCommand.CommandContent.Length >= command.CommnadLength)
                        {
                            var index = 0;
                            tempData.Vcc = BitConverter.ToSingle(resultCommand.CommandContent.AsSpan().Slice(index, 4).ToArray());
                            index += 4;
                            tempData.VcceRam = BitConverter.ToSingle(resultCommand.CommandContent.AsSpan().Slice(index, 4).ToArray());
                            index += 4;
                            tempData.VccrGxb = BitConverter.ToSingle(resultCommand.CommandContent.AsSpan().Slice(index, 4).ToArray());
                            index += 4;
                            tempData.VcctGxb = BitConverter.ToSingle(resultCommand.CommandContent.AsSpan().Slice(index, 4).ToArray());
                            index += 4;
                            tempData.TempBoard = BitConverter.ToSingle(resultCommand.CommandContent.AsSpan().Slice(index, 4).ToArray());
                            index += 4;
                            tempData.VccPt = BitConverter.ToSingle(resultCommand.CommandContent.AsSpan().Slice(index, 4).ToArray());
                            index += 4;
                            tempData.Vcc12V = BitConverter.ToSingle(resultCommand.CommandContent.AsSpan().Slice(index, 4).ToArray());
                            index += 4;
                            tempData.TempTsd = BitConverter.ToSingle(resultCommand.CommandContent.AsSpan().Slice(index, 4).ToArray());
                            index += 4;
                        }
                    }
                }
            }

            return result;
        }

        public static void SetPatternParam(string patternName = "")
        {
            var commonData = CommonData.Instance;
            var testItem = commonData.TestPlan?.TestItem?.FirstOrDefault(x => x.TestItemName.Equals(commonData.TestItemName));
            var args = commonData.TestItemArgs;

            if (args.Any() && string.IsNullOrEmpty(patternName))
                patternName = args.FirstOrDefault()?.ParamValue;

            if (string.IsNullOrEmpty(patternName))
                return;

            try
            {

                if (string.IsNullOrEmpty(patternName))
                    return;

                var patternFile = Instance?._patterns.FirstOrDefault(x => x.FileName.ToLower().Equals(patternName.ToLower()));
                if (patternFile == null)
                    return;

                var controlService = Instance?.ControlService;

                var dataStartAddress = patternFile.DataStartAddress;
                var dataLength = patternFile.PinDataLength;
                var pinList = patternFile.PatternVectors.FirstOrDefault()?.Pins;

                //  组装数据包
                var commandListDic = new Dictionary<int, List<CommandInfoModel>>();

                foreach (var pin in pinList)
                {
                    var channel = PinManagerHelper.GetPinByName(Instance.TestPlan?.Channel, pin.PinName);
                    var pinIndex = PinManagerHelper.GetPinIndexByPinName(Instance.TestPlan?.Channel, pin.PinName);
                    var dataEndAddress = dataStartAddress + dataLength - 1;
                    foreach (var site in channel.Sites)
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
                            var patternStartAddress = dataStartAddress;
                            var patternEndAddress = dataEndAddress;
                            var receiveStartAddress = (_maxChannelNum + pinIndex + 1) * _mbByte;
                            var receiveEndAddress = (_maxChannelNum + pinIndex + 2) * _mbByte;
                            var patternStartBytes = BitConverter.GetBytes(patternStartAddress);
                            var patternStopBytes = BitConverter.GetBytes(patternEndAddress);
                            var receiveStartBytes = BitConverter.GetBytes(receiveStartAddress);
                            var receiveStopBytes = BitConverter.GetBytes(receiveEndAddress);

                            var patternStartAddressBytes = new byte[5];
                            var patternStopAddressBytes = new byte[5];
                            Array.Copy(patternStartBytes, patternStartAddressBytes, patternStartAddressBytes.Length);
                            Array.Copy(patternStopBytes, patternStopAddressBytes, patternStopAddressBytes.Length);

                            var receiveStartAddressBytes = new byte[5];
                            var receiveStopAddressBytes = new byte[5];
                            Array.Copy(receiveStartBytes, receiveStartAddressBytes, receiveStartAddressBytes.Length);
                            Array.Copy(receiveStopBytes, receiveStopAddressBytes, receiveStopAddressBytes.Length);

                            var byteList = new List<byte>();
                            byteList.Add((byte)channelNum);
                            byteList.Add(1);
                            byteList.AddRange(patternStartAddressBytes.Reverse());
                            byteList.AddRange(patternStopAddressBytes.Reverse());
                            byteList.AddRange(receiveStartAddressBytes.Reverse());
                            byteList.AddRange(receiveStopAddressBytes.Reverse());
                            byteList.Add(0);    //  接收数据存入DDR
                            byteList.Add(4);    //  比特数
                            var command = new CommandInfoModel()
                            {
                                CommandCode = "0x0123",
                                CommandContent = byteList.ToArray(),
                            };
                            commandList.Add(command);
                        }
                    }
                    dataStartAddress = dataEndAddress;
                }

                if (commandListDic.Any())
                {
                    foreach (var commandList in commandListDic)
                    {
                        var slotNum = $"0x{commandList.Key.ToString("x2")}";
                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType.DPS, InstructionType.Configuration, commandList.Value);

                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType((BoardType)Instance?.BoardType, slotNum);

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

        public static void SetPatternFile(string[] patternFiles)
        {
            if (patternFiles == null || !patternFiles.Any())
                return;

            try
            {
                Instance?._patterns?.Clear();
                long lastPatternDataEndAddress = 0;
                foreach (string patternFile in patternFiles)
                {
                    if (!File.Exists(patternFile)) continue;
                    var pattern = PatternHelper.AnalysisPattern(patternFile);
                    Instance?._patterns.Add(pattern);
                    if (pattern != null)
                    {
                        pattern.DataStartAddress = lastPatternDataEndAddress;
                        var package = PatternHelper.ConversionPatternModel(pattern, ref lastPatternDataEndAddress, out int patternDataLength);
                        pattern.PinDataLength = patternDataLength;
                        pattern.DataEndAddress = lastPatternDataEndAddress;
                        if (package != null && package.Any())
                            SendPatternPackageToInstrument(package);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        internal static void SendPatternPackageToInstrument(List<PatternPackageModel> packages)
        {
            try
            {
                var controlService = Instance?.ControlService;

                foreach (var package in packages)
                {
                    var channel = PinManagerHelper.GetPinByName(Instance.TestPlan?.Channel, package.PinName);
                    if (channel == null)
                        continue;

                    foreach (var site in channel.Sites)
                    {
                        if (!ChannelManagerHelper.IsSiteValid(site.SiteName))
                            continue;

                        var channelNum = ChannelManagerHelper.GetChannelNumSiteInfo(site.SiteValue, out int slot);
                        if (channelNum >= 0)
                        {
                            var contentBytes = new byte[package.Length + _packageAdditionalLength];
                            contentBytes[0] = (byte)channelNum;
                            Array.Copy(package.Address, 0, contentBytes, 1, package.Address.Length);
                            Array.Copy(package.LengthBytes, 0, contentBytes, 1 + package.Address.Length, package.LengthBytes.Length);
                            var index = 1 + package.Address.Length + package.LengthBytes.Length;
                            foreach (var unit in package.PatternGroups)
                            {
                                contentBytes[index++] = (byte)unit.VectorNumber;
                                contentBytes[index++] = (byte)unit.Instruction;
                                if (unit.Parameter.Any())
                                {
                                    Array.Copy(unit.Parameter.ToArray(), 0, contentBytes, index, unit.Parameter.Count);
                                    index += unit.Parameter.Count;
                                }
                                Array.Copy(unit.Vectors.ToArray(), 0, contentBytes, index, unit.Vectors.Count);
                                index += unit.Vectors.Count;
                            }

                            var command = new CommandInfoModel()
                            {
                                CommandCode = "0x0124",
                                CommandContent = contentBytes.ToArray(),
                            };

                            var slotNum = $"0x{slot.ToString("x2")}";
                            var message = CommandHelper.GetCommandBytes(0xFF, BoardType.DPS, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                            var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType((BoardType)Instance?.BoardType, slotNum);

                            if (controlService != null && instrumentInfo != null)
                                controlService.Send(instrumentInfo, message);
                        }

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<PatternRunningStateModel> GetRunningState()
        {
            if (PinList == null || !PinList.Any())
                return null;

            var slotList = new List<int>();

            foreach (var pin in PinList)
            {
                var channel = PinManagerHelper.GetPinByName(Instance.TestPlan?.Channel, pin.PinName);
                foreach (var site in channel.Sites)
                {
                    if (!ChannelManagerHelper.IsSiteValid(site.SiteName))
                        continue;

                    var channelNum = ChannelManagerHelper.GetChannelNumSiteInfo(site.SiteValue, out int slot);
                    if (!slotList.Contains(slot))
                        slotList.Add(slot);
                }
            }
            var allChannelResult = GetAllChannelRunningState(slotList);
            var pinNames = PinList.Select(x => x.PinName);

            return allChannelResult.Where(x => !string.IsNullOrEmpty(x.PinName) && pinNames.Any(y => y.ToLower().Equals(x.PinName?.ToLower()))).ToList();
        }

        public static List<PatternRunningStateModel> GetAllChannelRunningState(List<int> slotList)
        {
            var result = new List<PatternRunningStateModel>();

            if (slotList == null || !slotList.Any())
                return result;

            try
            {
                var controlService = Instance?.ControlService;
                var instrumentInfos = new List<InstrumentBaseModel>();
                foreach (var slot in slotList)
                {
                    var tempInstumentInfos = InstrumentManagerHelper.GetInstrumentInfosByBoardType((BoardType)Instance?.BoardType, $"0x{slot.ToString("x2")}");
                    instrumentInfos.AddRange(tempInstumentInfos);
                }

                var sendCommand = new CommandInfoModel()
                {
                    CommandCode = "0x0125",
                    CommandContent = new byte[] { 255 }
                };

                if (controlService != null)
                {
                    foreach (var instrumentInfo in instrumentInfos)
                    {
                        var message = CommandHelper.GetCommandBytes((byte)instrumentInfo.SortId, BoardType.PE, InstructionType.Query, new List<CommandInfoModel>() { sendCommand });
                        var queryResult = controlService.Query(instrumentInfo, message);
                        var commands = CommandHelper.ConversionBytesToCommands(queryResult);

                        foreach (var command in commands)
                        {
                            if (command.CommnadLength >= 33)
                            {
                                var tempData = ContentToChannelRunningState(command.SlotNum, command.CommandContent);
                                result.AddRange(tempData);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }

        private static List<PatternRunningStateModel> ContentToChannelRunningState(int slot, byte[] commandContent)
        {
            var result = new List<PatternRunningStateModel>();

            try
            {
                if (commandContent.Any() && commandContent.Length >= 33)
                {
                    var passArray = commandContent.AsSpan().Slice(1, 16).ToArray().Reverse().ToArray();
                    var runningArray = commandContent.AsSpan().Slice(17, 16).ToArray().Reverse().ToArray();

                    var passResults = ByteArrayToBoolArray(passArray);
                    var runningResults = ByteArrayToBoolArray(runningArray);

                    if (passResults != null && passResults.Any())
                    {
                        var length = passResults.Length;

                        for (int i = 0; i < length; i++)
                        {
                            var tempData = new PatternRunningStateModel();
                            tempData.ChannelNum = i;
                            tempData.OriginalData = commandContent;
                            tempData.Site = ChannelManagerHelper.GetSiteInfo(slot, i);
                            tempData.PinName = PinManagerHelper.GetPinNameBySlotName(Instance.TestPlan?.Channel, tempData.Site);
                            tempData.SiteResult = passResults[i];
                            tempData.IsRunning = runningResults.Length > i ? runningResults[i] : false;

                            result.Add(tempData);
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        private static bool[] ByteArrayToBoolArray(byte[] bytes)
        {
            if (!bytes.Any())
                return null;

            var boolArray = new bool[bytes.Length * 8];
            // 遍历每个字节
            for (int i = 0; i < bytes.Length; i++)
            {
                var currentByte = bytes[i];
                // 遍历当前字节的每一位（从高位到低位）
                for (int j = 0; j < 8; j++)
                {
                    // 提取当前位
                    var bit = (currentByte & (1 << j)) != 0;
                    // 将布尔值存储到结果数组中
                    boolArray[i * 8 + j] = bit;
                }
            }
            return boolArray;
        }

        public List<ChannelResultModel<int>> GetFailPosition()
        {
            var result = new List<ChannelResultModel<int>>();
            if (PinList == null || !PinList.Any())
                return result;

            try
            {
                //  组装数据包
                var commandListDic = new Dictionary<int, List<CommandInfoModel>>();

                var controlService = Instance?.ControlService;

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
                            var command = new CommandInfoModel()
                            {
                                CommandCode = "0x0126",
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
                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType.DPS, InstructionType.Query, commandList.Value);

                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(BoardType, slotNum);

                        if (controlService != null && instrumentInfo != null)
                        {
                            var queryResult = controlService.Query(instrumentInfo, message);
                            var commands = CommandHelper.ConversionBytesToCommands(queryResult);

                            foreach (var command in commands)
                            {
                                if (command.CommandContent.Length >= 5)
                                {
                                    var tempData = new ChannelResultModel<int>();
                                    tempData.ChannelNum = (int)command.CommandContent[0];
                                    tempData.OriginalData = command.CommandContent;
                                    if (tempData.ChannelNum >= 0)
                                    {
                                        tempData.Site = ChannelManagerHelper.GetSiteInfo(command.SlotNum, tempData.ChannelNum);
                                        tempData.PinName = PinManagerHelper.GetPinNameBySlotName(Instance.TestPlan?.Channel, tempData.Site);
                                    }
                                    tempData.SiteResult = BitConverter.ToInt32(command.CommandContent.AsSpan(1, 4).ToArray().Reverse().ToArray());  //存在疑问，未调试
                                    result.Add(tempData);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        public List<ChannelResultModel<PatternResultModel>> GetResult(ushort startAddress, short length)
        {
            var result = new List<ChannelResultModel<PatternResultModel>>();
            if (PinList == null || !PinList.Any())
                return result;

            try
            {
                //  组装数据包
                var commandListDic = new Dictionary<int, List<CommandInfoModel>>();

                var controlService = Instance?.ControlService;

                var addressBytes = BitConverter.GetBytes(startAddress).Reverse().ToArray();
                var lengthBytes = BitConverter.GetBytes(length).Reverse().ToArray();

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
                            byteList.AddRange(addressBytes);
                            byteList.AddRange(lengthBytes);

                            var command = new CommandInfoModel()
                            {
                                CommandCode = "0x0127",
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
                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType.DPS, InstructionType.Query, commandList.Value);

                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(BoardType, slotNum);

                        if (controlService != null && instrumentInfo != null)
                        {
                            var queryResult = controlService.Query(instrumentInfo, message);
                            var commands = CommandHelper.ConversionBytesToCommands(queryResult);

                            foreach (var command in commands)
                            {
                                if (command.CommnadLength >= length + 5)
                                {
                                    var tempData = ContentToChannelResult(command.SlotNum, command.CommandContent);
                                    result.Add(tempData);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        private ChannelResultModel<PatternResultModel> ContentToChannelResult(int slot, byte[] commandContent)
        {
            var result = new ChannelResultModel<PatternResultModel>();

            try
            {
                result.ChannelNum = (int)commandContent[0];
                result.OriginalData = commandContent;
                if (result.ChannelNum >= 0)
                {
                    result.Site = ChannelManagerHelper.GetSiteInfo(slot, result.ChannelNum);
                    result.PinName = PinManagerHelper.GetPinNameBySlotName(Instance.TestPlan?.Channel, result.Site);
                }
                result.SiteResult = new PatternResultModel();

                var startAddressBytes = commandContent.AsSpan(1, 2).ToArray().Reverse().ToArray();
                var lengthBytes = commandContent.AsSpan(2, 2).ToArray().Reverse().ToArray();
                result.SiteResult.StartAddress = System.Convert.ToUInt16(startAddressBytes);
                result.SiteResult.Length = System.Convert.ToInt16(lengthBytes);
                result.SiteResult.Data = commandContent.AsSpan(5).ToArray();
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        public List<ChannelResultModel<PatternStorageAddress>> GetStorageAddress()
        {
            if (PinList == null || !PinList.Any())
                return null;

            var result = new List<ChannelResultModel<PatternStorageAddress>>();

            try
            {
                //  组装数据包
                var commandListDic = new Dictionary<int, List<CommandInfoModel>>();

                var controlService = Instance?.ControlService;

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

                            var command = new CommandInfoModel()
                            {
                                CommandCode = "0x0128",
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
                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType.DPS, InstructionType.Query, commandList.Value);

                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(BoardType, slotNum);

                        if (controlService != null && instrumentInfo != null)
                        {
                            var queryResult = controlService.Query(instrumentInfo, message);
                            var commands = CommandHelper.ConversionBytesToCommands(queryResult);

                            foreach (var command in commands)
                            {
                                if (command.CommnadLength >= 11)
                                {
                                    var tempData = new ChannelResultModel<PatternStorageAddress>();
                                    tempData.ChannelNum = (int)command.CommandContent[0];
                                    tempData.OriginalData = command.CommandContent;
                                    if (tempData.ChannelNum >= 0)
                                    {
                                        tempData.Site = ChannelManagerHelper.GetSiteInfo(command.SlotNum, tempData.ChannelNum);
                                        tempData.PinName = PinManagerHelper.GetPinNameBySlotName(TestPlan?.Channel, tempData.Site);
                                    }
                                    var buffAddressBytes = new byte[8];
                                    tempData.SiteResult = new PatternStorageAddress();
                                    var startAddressBytes = command.CommandContent.AsSpan(1, 5).ToArray().Reverse().ToArray();
                                    Array.Copy(startAddressBytes, buffAddressBytes, 5);
                                    tempData.SiteResult.StartAddress = BitConverter.ToInt64(buffAddressBytes);

                                    var endAddressBytes = command.CommandContent.AsSpan(6, 5).ToArray().Reverse().ToArray();
                                    Array.Copy(endAddressBytes, buffAddressBytes, 5);
                                    tempData.SiteResult.EndAddress = BitConverter.ToInt64(buffAddressBytes);
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
    }
}
