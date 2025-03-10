using KSW.ATE01.Instrument.IO.BLLs.Abstractions.Digitals;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Instruments;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Patterns;
using KSW.ATE01.Instrument.IO.Enums.Instruments;
using KSW.ATE01.Instrument.IO.Helpers;
using KSW.ATE01.Instrument.IO.Models.Instruments;
using KSW.ATE01.Project.Base.Models;
using System.Collections.Generic;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.Digitals
{
    public class Digital : PE131CommandBase<Digital>, IDigital
    {
        private const double _periodResolution = 6.25e-10;
        private const double _ns = 1e-9;

        /// <summary>
        /// 设置周期
        /// </summary>
        /// <param name="pwa_en">-128~+127</param>
        /// <param name="cd_en">0~63</param>
        /// <param name="fd_en">0~63</param>
        /// <param name="pwa_d">-128~+127</param>
        /// <param name="cd_d">0~63</param>
        /// <param name="fd_d">0~63</param>
        /// <param name="pwa_ca">-128~+127</param>
        /// <param name="cd_ca">0~63</param>
        /// <param name="fd_ca">0~63</param>
        /// <param name="pwa_cb">-128~+127</param>
        /// <param name="cd_cb">0~63</param>
        /// <param name="fd_cb">0~63</param>
        public static void SetTiming(sbyte pwa_en = 0, byte cd_en = 0, ushort fd_en = 0, sbyte pwa_d = 0, byte cd_d = 0, ushort fd_d = 0, sbyte pwa_ca = 0, byte cd_ca = 0, ushort fd_ca = 0, sbyte pwa_cb = 0, byte cd_cb = 0, ushort fd_cb = 0, byte d_d_d = 0, byte en_d_d = 0, byte ca_d_d = 0, byte cb_d_d = 0, short cab_d_c = 0)
        {
            if (cd_en < 0 || cd_en > 63)
                throw new ArgumentOutOfRangeException(nameof(cd_en), "取值范围:0~ 63");

            if (fd_en < 0 || fd_en > 63)
                throw new ArgumentOutOfRangeException(nameof(fd_en), "取值范围:0~ 63");

            if (cd_d < 0 || cd_d > 63)
                throw new ArgumentOutOfRangeException(nameof(cd_d), "取值范围:0~ 63");

            if (fd_d < 0 || fd_d > 63)
                throw new ArgumentOutOfRangeException(nameof(fd_d), "取值范围:0~ 63");

            if (cd_ca < 0 || cd_ca > 63)
                throw new ArgumentOutOfRangeException(nameof(cd_ca), "取值范围:0~ 63");

            if (fd_ca < 0 || fd_ca > 63)
                throw new ArgumentOutOfRangeException(nameof(fd_ca), "取值范围:0~ 63");

            if (cd_cb < 0 || cd_cb > 63)
                throw new ArgumentOutOfRangeException(nameof(cd_cb), "取值范围:0~ 63");

            if (fd_cb < 0 || fd_cb > 63)
                throw new ArgumentOutOfRangeException(nameof(fd_cb), "取值范围:0~ 63");

            var commonData = CommonData.Instance;
            if (commonData == null || commonData.TestPlan == null || commonData.TestPlan?.Channel == null || commonData.TestPlan?.TestItem == null)
                return;


            if (!string.IsNullOrEmpty(commonData.Timing))
            {
                SetTimingDetail(pwa_en, cd_en, fd_en, pwa_d, cd_d, fd_d, pwa_ca, cd_ca, fd_ca, pwa_cb, cd_cb, fd_cb, d_d_d, en_d_d, ca_d_d, cb_d_d, cab_d_c);
            }
        }

        private static void SetTimingDetail(sbyte pwa_en, byte cd_en, ushort fd_en, sbyte pwa_d, byte cd_d, ushort fd_d, sbyte pwa_ca, byte cd_ca, ushort fd_ca, sbyte pwa_cb, byte cd_cb, ushort fd_cb, byte d_d_d, byte en_d_d, byte ca_d_d, byte cb_d_d, short cab_d_c)
        {
            var commonData = CommonData.Instance;

            var testItem = commonData.TestPlan?.TestItem?.FirstOrDefault(x => x.TestItemName.Equals(commonData.TestItemName));
            var args = commonData.TestItemArgs;

            if (args.Any())
            {
                var patternFileName = args.FirstOrDefault()?.ParamValue;
                var patterns = Pattern.Instance.Patterns;
                var patternFile = patterns.FirstOrDefault(x => x.PatternFileName.Equals(patternFileName));
                var timings = testItem?.Timings;
                if (patternFile != null)
                {
                    var patternTimings = timings?.Where(x => patternFile.TimingSets.Contains(x.TimingName));

                    if (patternTimings.Any())
                    {
                        try
                        {
                            var controlService = Instance.ControlService;

                            foreach (var patternTiming in patternTimings)
                            {
                                var pinList = commonData.TestPlan.Channel?.Where(x => x.Groups.Any(y => y.Name.Equals(patternTiming.PinName))).ToList();
                                var period = patternTiming.Period * _ns;

                                if (period < 0 || period > 0.02684354559375)
                                    throw new ArgumentOutOfRangeException("Period", "取值范围:0~0.02684354559375");

                                var periodUInt = Convert.ToUInt32(period / _periodResolution);
                                var periodBytes = BitConverter.GetBytes(periodUInt).Reverse();

                                double driveA = 0;
                                double.TryParse(patternTiming.DriveA, out driveA);
                                driveA *= _ns;
                                var driveAUInt = Convert.ToUInt32(driveA / _periodResolution);
                                var driveABytes = BitConverter.GetBytes(driveAUInt).Reverse();

                                double driveB = 0;
                                double.TryParse(patternTiming.DriveB, out driveB);
                                driveB *= _ns;
                                var driveBUInt = Convert.ToUInt32(driveB / _periodResolution);
                                var driveBBytes = BitConverter.GetBytes(driveBUInt).Reverse();

                                double driveC = 0;
                                double.TryParse(patternTiming.DriveC, out driveC);
                                driveC *= _ns;
                                var driveCUInt = Convert.ToUInt32(driveC / _periodResolution);
                                var driveCBytes = BitConverter.GetBytes(driveCUInt).Reverse();

                                double driveD = 0;
                                double.TryParse(patternTiming.DriveD, out driveD);
                                driveD *= _ns;
                                var driveDUInt = Convert.ToUInt32(driveD / _periodResolution);
                                var driveDBytes = BitConverter.GetBytes(driveDUInt).Reverse();

                                var formatByte = Convert.ToByte(patternTiming.Fmt);
                                var strobeByte = Convert.ToByte(patternTiming.StrobeMode);

                                double r0Period = patternTiming.StrobeA * _ns;
                                var r0PeriodUInt = Convert.ToUInt32(r0Period / _periodResolution);
                                var r0PeriodBytes = BitConverter.GetBytes(r0PeriodUInt).Reverse();

                                double r1Period = patternTiming.StrobeB * _ns;
                                var r1PeriodUInt = Convert.ToUInt32(r1Period / _periodResolution);
                                var r1PeriodBytes = BitConverter.GetBytes(r1PeriodUInt).Reverse();

                                var fd_enBytes = BitConverter.GetBytes(fd_en).Reverse();
                                var fd_dBytes = BitConverter.GetBytes(fd_d).Reverse();
                                var fd_caBytes = BitConverter.GetBytes(fd_ca).Reverse();
                                var fd_cbBytes = BitConverter.GetBytes(fd_cb).Reverse();
                                var cab_dcBytes = BitConverter.GetBytes(cab_d_c).Reverse();
                                var commandListDic = new Dictionary<int, List<CommandInfoModel>>();

                                foreach (var pin in pinList)
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
                                            byteList.Add((byte)channelNum);        //暂时按顺序下发通道（后续需要映射站点信息）
                                            byteList.AddRange(periodBytes);
                                            byteList.Add(formatByte);
                                            byteList.Add(strobeByte);
                                            byteList.AddRange(driveABytes);   //D0
                                            byteList.AddRange(driveBBytes);   //D1
                                            byteList.AddRange(driveCBytes);   //D2
                                            byteList.AddRange(driveDBytes);   //D3
                                            byteList.AddRange(r0PeriodBytes); //R0
                                            byteList.AddRange(r1PeriodBytes); //R1
                                            byteList.Add((byte)pwa_en);       //pwa_en
                                            byteList.Add(cd_en);              //cd_en
                                            byteList.AddRange(fd_enBytes);    //fd_en
                                            byteList.Add((byte)pwa_d);        //pwa_d
                                            byteList.Add(cd_d);               //cd_d
                                            byteList.AddRange(fd_dBytes);     //fd_d
                                            byteList.Add((byte)pwa_ca);       //pwa_ca
                                            byteList.Add(cd_ca);              //cd_ca
                                            byteList.AddRange(fd_caBytes);    //fd_ca
                                            byteList.Add((byte)pwa_cb);       //pwa_cb
                                            byteList.Add(cd_cb);              //cd_cb
                                            byteList.AddRange(fd_cbBytes);    //fd_cb
                                            byteList.Add(d_d_d);              //d_d_d
                                            byteList.Add(en_d_d);              //en_d_d
                                            byteList.Add(ca_d_d);              //ca_d_d
                                            byteList.Add(cb_d_d);              //cb_d_d
                                            byteList.AddRange(cab_dcBytes);    //cab_d_c
                                            var command = new CommandInfoModel()
                                            {
                                                CommandCode = "0x010A",
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

                }
            }
        }
    }
}
