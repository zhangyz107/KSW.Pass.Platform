using KSW.ATE01.Instrument.IO.BLLs.Abstractions.PE;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Instruments;
using KSW.ATE01.Instrument.IO.Enums.PE;
using KSW.ATE01.Instrument.IO.Helpers;
using KSW.ATE01.Instrument.IO.Models.Instruments;
using KSW.ATE01.Project.Base.Enums.Errors;
using KSW.ATE01.Project.Base.Models.Errors;
using KSW.ATE01.Project.Base.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.PE
{
    public class Pe : PE131CommandBase<Pe>, IPe
    {
        public static IPe Pins(string pinList)
        {
            if (string.IsNullOrEmpty(pinList))
                throw new ArgumentNullException(nameof(pinList));

            Instance.GetPinList(pinList);

            return Instance;
        }

        public void SetWorkMode(TRXMode trxMode, TERMode terMode)
        {
            var location = nameof(SetWorkMode);

            if (PinList == null || !PinList.Any())
                return;

            if (Message.ErrorStatus != ErrorStatus.Error)
            {
                Message.ModuleName = nameof(PE);
                Message.FunctionName = nameof(SetWorkMode);

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
                                byteList.Add((byte)channelNum);     //暂时按顺序下发通道（后续需要映射站点信息）
                                byteList.Add(System.Convert.ToByte(trxMode));
                                byteList.Add(System.Convert.ToByte(terMode));

                                var command = new CommandInfoModel()
                                {
                                    CommandCode = "0x0001",
                                    CommandContent = byteList.ToArray(),
                                };
                                commandList.Add(command);
                            }
                        }
                    }

                    if (commandListDic.Any())
                    {
                        foreach (var coomandList in commandListDic)
                        {
                            var slotNum = $"0x{coomandList.Key.ToString("x2")}";
                            var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, slotNum);

                            if (instrumentInfo == null)
                                continue;

                            var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? Enums.Instruments.BoardType.Unknown, Enums.Instruments.InstructionType.Configuration, coomandList.Value);


                            if (controlService != null && instrumentInfo != null)
                                controlService.Send(instrumentInfo, message);
                        }
                    }
                }
                catch (ATEException)
                {

                    throw;
                }
                catch (Exception inner)
                {
                    ErrorMessages.IO.InternalError(inner, location);
                }
            }
        }

        public void SetHSWorkVoltage(double voh, double iout)
        {
            var location = nameof(SetHSWorkVoltage);

            if (PinList == null || !PinList.Any())
                return;

            if (Message.ErrorStatus != ErrorStatus.Error)
            {
                Message.ModuleName = nameof(PE);
                Message.FunctionName = nameof(SetHSWorkVoltage);

                try
                {
                    if (voh < -1.33 || voh > 1.22)
                        ErrorMessages.Pe.WorkVoltageOutOfRange(nameof(voh), new object[] { voh, "-1.33V", "1.22V" });

                    if (iout < 0 || iout > 0.01275)
                        ErrorMessages.Pe.WorkVoltageOutOfRange(nameof(iout), new object[] { iout, "0", "0.01275" });

                    var vohByte = (byte)((voh + 0.05) * 100);
                    var ioutByte = (byte)((iout / 50) * 1000000);

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
                                byteList.Add((byte)channelNum);     //暂时按顺序下发通道（后续需要映射站点信息）
                                byteList.Add(vohByte);
                                byteList.Add(ioutByte);

                                var command = new CommandInfoModel()
                                {
                                    CommandCode = "0x0002",
                                    CommandContent = byteList.ToArray(),
                                };
                                commandList.Add(command);
                            }
                        }
                    }

                    if (commandListDic.Any())
                    {
                        foreach (var coomandList in commandListDic)
                        {
                            var slotNum = $"0x{coomandList.Key.ToString("x2")}";
                            var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, slotNum);

                            if (instrumentInfo == null)
                                continue;

                            var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? Enums.Instruments.BoardType.Unknown, Enums.Instruments.InstructionType.Configuration, coomandList.Value);


                            if (controlService != null && instrumentInfo != null)
                                controlService.Send(instrumentInfo, message);
                        }
                    }

                }
                catch (ATEException)
                {

                    throw;
                }
                catch (Exception inner)
                {
                    ErrorMessages.IO.InternalError(inner, location);
                }
            }
        }

        public void SetLPWorkVoltage(double f0, double f1, double cv)
        {
            var location = nameof(SetLPWorkVoltage);

            if (PinList == null || !PinList.Any())
                return;

            if (Message.ErrorStatus != ErrorStatus.Error)
            {
                Message.ModuleName = nameof(PE);
                Message.FunctionName = nameof(SetLPWorkVoltage);

                try
                {
                    if (f0 < -7.048592 || f0 > 2.388448)
                        ErrorMessages.Pe.WorkVoltageOutOfRange(nameof(f0), new object[] { f0, "-7.048592", "2.388448" });

                    if (f1 < -7.048592 || f1 > 2.388448)
                        ErrorMessages.Pe.WorkVoltageOutOfRange(nameof(f1), new object[] { f1, "-7.048592", "2.388448" });

                    if (cv < -7.048592 || cv > 2.388448)
                        ErrorMessages.Pe.WorkVoltageOutOfRange(nameof(cv), new object[] { cv, "-7.048592", "2.388448" });

                    var f0Value = GetUshortValue(f0);
                    var f0Bytes = BitConverter.GetBytes(f0Value).Reverse();
                    var f1Value = GetUshortValue(f1);
                    var f1Bytes = BitConverter.GetBytes(f1Value).Reverse();
                    var cvValue = GetUshortValue(cv);
                    var cvBytes = BitConverter.GetBytes(cvValue).Reverse();

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
                                byteList.Add((byte)channelNum);     //暂时按顺序下发通道（后续需要映射站点信息）
                                byteList.AddRange(f0Bytes);
                                byteList.AddRange(f1Bytes);
                                byteList.AddRange(cvBytes);

                                var command = new CommandInfoModel()
                                {
                                    CommandCode = "0x0004",
                                    CommandContent = byteList.ToArray(),
                                };
                                commandList.Add(command);
                            }
                        }
                    }

                    if (commandListDic.Any())
                    {
                        foreach (var coomandList in commandListDic)
                        {
                            var slotNum = $"0x{coomandList.Key.ToString("x2")}";
                            var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, slotNum);

                            if (instrumentInfo == null)
                                continue;

                            var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? Enums.Instruments.BoardType.Unknown, Enums.Instruments.InstructionType.Configuration, coomandList.Value);


                            if (controlService != null && instrumentInfo != null)
                                controlService.Send(instrumentInfo, message);
                        }
                    }

                }
                catch (ATEException)
                {

                    throw;
                }
                catch (Exception inner)
                {
                    ErrorMessages.IO.InternalError(inner, location);
                }
            }
        }


        private ushort GetUshortValue(double v)
        {
            var tempValue = (v + 2.33) / 144 * 1000000;
            return System.Convert.ToUInt16(Math.Floor(tempValue));
        }

    }
}
