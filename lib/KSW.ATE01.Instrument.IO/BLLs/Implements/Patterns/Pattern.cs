using KSW.ATE01.Instrument.IO.BLLs.Abstractions.Patterns;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Instruments;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Results;
using KSW.ATE01.Instrument.IO.Enums.Instruments;
using KSW.ATE01.Instrument.IO.Enums.Patterns;
using KSW.ATE01.Instrument.IO.Helpers;
using KSW.ATE01.Instrument.IO.Models.Instruments;
using KSW.ATE01.Instrument.IO.Models.Results;
using KSW.ATE01.Project.Base.Enums.Errors;
using KSW.ATE01.Project.Base.Enums.Patterns;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Models;
using KSW.ATE01.Project.Base.Models.Errors;
using KSW.ATE01.Project.Base.Models.Exceptions;
using KSW.ATE01.Project.Base.Models.Patterns;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Packaging;
using System.Text;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.Patterns
{
    public class Pattern : PE131CommandBase<Pattern>, IPattern
    {
        private const long _mbByte = 128 * 1024 * 1024L;
        private const int _maxChannelNum = 127;
        private const int _packageAdditionalLength = 8;
        private static ushort _patternUnitLength = 4 * 1024 * 8;    // Pattern单位长度(61440)

        //private List<PatternModel> _patterns = new List<PatternModel>();
        private List<BinPatternModel> _binPatterns = new List<BinPatternModel>();

        //public List<PatternModel> Patterns { get => _patterns; }

        public List<BinPatternModel> BinPatterns { get => _binPatterns; }

        public static IPattern Pins(string pinList)
        {
            if (string.IsNullOrEmpty(pinList))
                throw new ArgumentNullException(nameof(pinList));

            Instance.GetPinList(pinList);

            return Instance;
        }

        public void SetPinType(PinIOType pinIOType)
        {
            if (PinList == null || !PinList.Any())
                return;

            if (Message.ErrorStatus != ErrorStatus.Error)
            {
                Message.ModuleName = "Pattern";
                Message.FunctionName = "Set Pin Type";
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
                                byteList.Add((byte)channelNum);        //暂时按顺序下发通道（后续需要映射站点信息）
                                byteList.Add(System.Convert.ToByte(pinIOType));    //Type

                                var command = new CommandInfoModel()
                                {
                                    CommandCode = "0x0108",
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
                catch (Exception ex)
                {
                    ErrorMessages.InsGeneral.MarkerError($"{nameof(Pattern)}.{nameof(SetPinType)}", new object[]
                    {
                        ex.Message
                    });
                }
            }
        }

        public void SetPinInit(PinInitVoltageType pinInitVoltageType)
        {
            var location = nameof(SetPinInit);

            if (PinList == null || !PinList.Any())
                return;

            if (Message.ErrorStatus != ErrorStatus.Error)
            {
                Message.ModuleName = "Pattern";
                Message.FunctionName = "Set Pin Init";
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
                                byteList.Add(System.Convert.ToByte(pinInitVoltageType)); //Type

                                var command = new CommandInfoModel()
                                {
                                    CommandCode = "0x0109",
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

                var patternFile = Instance?.BinPatterns.FirstOrDefault(x => x.VectorName.ToLower().Equals(patternName.ToLower()));
                if (patternFile == null)
                    return;

                var controlService = Instance?.ControlService;

                var dataStartAddress = patternFile.DataStartAddress;
                var dataLength = patternFile.PinDataLength;
                var pinList = patternFile.PinPacks.Where(x => !string.IsNullOrEmpty(x.PinName))?.Select(x => x.PinName);

                //  组装数据包
                var commandListDic = new Dictionary<int, List<CommandInfoModel>>();

                foreach (var pin in pinList)
                {
                    var channel = PinManagerHelper.GetPinByName(Instance.TestPlan?.Channel, pin);
                    var pinIndex = PinManagerHelper.GetPinIndexByPinName(Instance.TestPlan?.Channel, pin);
                    var dataEndAddress = dataStartAddress + dataLength - 1;
                    if (channel?.Sites?.Any() == true)
                    {
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
                                    CommandCode = "0x010B",
                                    CommandContent = byteList.ToArray(),
                                };
                                commandList.Add(command);
                            }
                        }
                    }
                    dataStartAddress = dataEndAddress + 1;
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

            catch (Exception)
            {

                throw;
            }
        }

        //public static void SetPatternFile(string[] patternFiles)
        //{
        //    if (patternFiles == null || !patternFiles.Any())
        //        return;

        //    try
        //    {
        //        Instance?._binPatterns?.Clear();
        //        long lastPatternDataEndAddress = 0;
        //        foreach (string patternFile in patternFiles)
        //        {
        //            if (!File.Exists(patternFile)) continue;
        //            var groupQueue = new ConcurrentQueue<PatternVectorGroupModel>();
        //            var pattern = PatternReaderWriterHelper.ReadPattern(patternFile);
        //            Instance?._binPatterns.Add(pattern);
        //            if (pattern != null)
        //            {
        //                pattern.DataStartAddress = lastPatternDataEndAddress;
        //                var package = PatternHelper.ConversionPatternModel(pattern, ref lastPatternDataEndAddress);
        //                pattern.DataEndAddress = lastPatternDataEndAddress;
        //                if (package != null && package.Any())
        //                    SendPatternPackageToInstrument(package);
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        public static void SetPatternFile(string[] patternFiles)
        {
            if (patternFiles == null || !patternFiles.Any())
                return;

            try
            {
                Instance?._binPatterns?.Clear();
                long lastPatternDataEndAddress = 0;
                var groups = new BlockingCollection<PatternVectorGroupModel>(boundedCapacity: 12800);
                var lengthBytes = BitConverter.GetBytes(_patternUnitLength).Reverse().ToArray();
                var onePackageVectorCount = _patternUnitLength / 64; //一个udp包最大字节数（61440）/单个向量字节数
                foreach (string patternFile in patternFiles)
                {
                    if (!File.Exists(patternFile)) continue;
                    using (var fs = new FileStream(patternFile, FileMode.Open))
                    {
                        using (var br = new BinaryReader(fs))
                        {
                            try
                            {
                                var pattern = ReadPatternHeader(br, patternFile);
                                var readTask = Task.Run(async () => await ReadPatternVectors(br, groups));
                                var pinDataLength = 0;
                                Instance?._binPatterns.Add(pattern);
                                if (pattern != null)
                                {
                                    pattern.DataStartAddress = lastPatternDataEndAddress;
                                    var groupUnit = new List<PatternVectorGroupModel>();
                                    var lastPinName = string.Empty;
                                    foreach (var group in groups.GetConsumingEnumerable())
                                    {
                                        if (lastPinName.Equals(string.Empty))
                                            lastPinName = group.PinName;
                                        else if (!lastPinName.Equals(group.PinName))
                                        {
                                            //var packageModel = new PatternPackageStruct();
                                            //packageModel.PinName = lastPinName;
                                            //var addr = lastPatternDataEndAddress;
                                            //packageModel.Address = BitConverter.GetBytes(addr).Reverse().Skip(3).ToArray();
                                            //packageModel.LengthBytes = lengthBytes;
                                            //lastPatternDataEndAddress += _patternUnitLength;

                                            //if (groupUnit.Count < 64)   //一个打包包含64个向量组 4096 / 64 = 64
                                            //{
                                            //    var rest = 64 - groupUnit.Count;
                                            //    for (int i = 0; i < rest; i++)
                                            //        groupUnit.Add(new PatternVectorGroupModel());   //填充空的数据包
                                            //}
                                            //packageModel.PatternGroups.AddRange(groupUnit);
                                            //SendOnePackageToInstrument(packageModel);
                                            //groupUnit.Clear();

                                            PackOnePackageData(ref lastPatternDataEndAddress, lengthBytes, ref pinDataLength, groupUnit, lastPinName);

                                            lastPinName = group.PinName;
                                            pinDataLength = 0;
                                        }

                                        groupUnit.Add(group);

                                        if (groupUnit.Count % onePackageVectorCount == 0)
                                        {
                                            PackOnePackageData(ref lastPatternDataEndAddress, lengthBytes, ref pinDataLength, groupUnit, lastPinName);
                                            //var packageModel = new PatternPackageStruct();
                                            //packageModel.PinName = lastPinName;
                                            //var addr = lastPatternDataEndAddress;
                                            //packageModel.Address = BitConverter.GetBytes(addr).Reverse().Skip(3).ToArray();
                                            //packageModel.LengthBytes = lengthBytes;
                                            //lastPatternDataEndAddress += _patternUnitLength;
                                            //pinDataLength += _patternUnitLength;
                                            //packageModel.PatternGroups.AddRange(groupUnit);
                                            //SendOnePackageToInstrument(packageModel);
                                            //groupUnit.Clear();
                                        }
                                    }

                                    if (groupUnit.Any())    //处理最后剩余的数据
                                    {
                                        PackOnePackageData(ref lastPatternDataEndAddress, lengthBytes, ref pinDataLength, groupUnit, lastPinName);
                                    }
                                    pattern.PinDataLength = pinDataLength;
                                    pattern.DataEndAddress = lastPatternDataEndAddress;
                                }
                            }
                            catch (Exception ex)
                            {
                                LogHelper.WriteError(ex.Message);
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static BinPatternModel ReadPatternHeader(BinaryReader br, string patternFile)
        {
            var result = new BinPatternModel();
            result.PinPacks = new List<PinPackModel>();

            if (File.Exists(patternFile))
            {
                result.FileName = Path.GetFileName(patternFile);
                result.FilePath = patternFile;
                result.VectorName = Path.GetFileNameWithoutExtension(patternFile);

                br.BaseStream.Position = 0;
                result.ModuleType = (ModuleType)br.ReadByte();
                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    var pack = new PinPackModel();
                    var nameLength = br.ReadInt32();
                    var nameBytes = br.ReadBytes(nameLength);
                    pack.PinName = Encoding.UTF8.GetString(nameBytes);
                    var timingSetLength = br.ReadInt32();
                    if (timingSetLength > 0)
                    {
                        var timingSetBytes = br.ReadBytes(timingSetLength);
                        pack.TimingSet = Encoding.UTF8.GetString(timingSetBytes);
                    }
                    var length = br.ReadInt64();
                    br.BaseStream.Position += length;
                    //pack.Data = new List<PatternVectorGroupModel>();
                    result.PinPacks.Add(pack);
                }
            }
            return result;
        }

        private static async Task ReadPatternVectors(BinaryReader br, BlockingCollection<PatternVectorGroupModel> groupQueue)
        {
            br.BaseStream.Position = 0;
            br.ReadByte();  //ModuleType
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                var nameLength = br.ReadInt32();
                var nameBytes = br.ReadBytes(nameLength);
                var pinName = Encoding.UTF8.GetString(nameBytes);
                var timingSetLength = br.ReadInt32();
                if (timingSetLength > 0)
                    br.ReadBytes(timingSetLength);  // TimingSetBytes
                var length = br.ReadInt64();
                while (length > 0)
                {
                    var group = new PatternVectorGroupModel();
                    group.PinName = pinName;
                    group.VectorNumber = br.ReadByte();
                    group.Instruction = br.ReadByte();
                    group.Data = br.ReadBytes(62);
                    groupQueue.Add(group);
                    length -= 64;
                }
            }

            groupQueue.CompleteAdding();
            return;
        }

        private static void PackOnePackageData(ref long lastPatternDataEndAddress, byte[] lengthBytes, ref int pinDataLength, List<PatternVectorGroupModel> groupUnit, string lastPinName)
        {
            var packageModel = new PatternPackageStruct();
            packageModel.PinName = lastPinName;
            var addr = lastPatternDataEndAddress;
            packageModel.Address = BitConverter.GetBytes(addr).Reverse().Skip(3).ToArray();
            packageModel.Length = _patternUnitLength;
            packageModel.LengthBytes = lengthBytes;
            lastPatternDataEndAddress += _patternUnitLength;
            pinDataLength += _patternUnitLength;
            var vectorGroupCount = _patternUnitLength / 64;  //一个打包包含512个向量组 4096 * 8 / 64 = 512
            if (groupUnit.Count < vectorGroupCount)
            {
                var rest = vectorGroupCount - groupUnit.Count;
                for (int i = 0; i < rest; i++)
                    groupUnit.Add(new PatternVectorGroupModel());   //填充空的数据包
            }
            packageModel.PatternGroups.AddRange(groupUnit);
            SendOnePackageToInstrument(packageModel);
            groupUnit.Clear();
        }


        private static bool SendOnePackageToInstrument(PatternPackageStruct package)
        {
            var controlService = Instance?.ControlService;
            var channel = PinManagerHelper.GetPinByName(Instance.TestPlan?.Channel, package.PinName);
            if (channel == null)
                return false;

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
                        var instruction = (byte)unit.Instruction;
                        contentBytes[index++] = unit.VectorNumber;
                        contentBytes[index++] = (byte)(instruction << 1);
                        Array.Copy(unit.Data, 0, contentBytes, index, unit.Data.Length);
                        index += unit.Data.Length;
                    }

                    var command = new CommandInfoModel()
                    {
                        CommandCode = "0x010C",
                        CommandContent = contentBytes.ToArray(),
                    };

                    var slotNum = $"0x{slot.ToString("x2")}";
                    var message = CommandHelper.GetCommandBytes(0xFF, BoardType.PE, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType((BoardType)Instance?.BoardType, slotNum);

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }

            return true;
        }

        private static void SendPatternPackageToInstrument(List<PatternPackageModel> packages)
        {
            try
            {
                var controlService = Instance?.ControlService;

                if (packages == null || !packages.Any())
                    return;

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
                                var instruction = (byte)unit.Instruction;
                                contentBytes[index++] = (byte)unit.VectorNumber;
                                contentBytes[index++] = (byte)(instruction << 1);
                                if (unit.Parameter.Any())
                                {
                                    var parameterLength = unit.Parameter.Count > 6 ? 6 : unit.Parameter.Count;
                                    Array.Copy(unit.Parameter.ToArray(), 0, contentBytes, index, parameterLength);
                                    index += 6;
                                }
                                Array.Copy(unit.Vectors.ToArray(), 0, contentBytes, index, unit.Vectors.Count);
                                index += unit.Vectors.Count;
                            }

                            var command = new CommandInfoModel()
                            {
                                CommandCode = "0x010C",
                                CommandContent = contentBytes.ToArray(),
                            };

                            var slotNum = $"0x{slot.ToString("x2")}";
                            var message = CommandHelper.GetCommandBytes(0xFF, BoardType.PE, InstructionType.Configuration, new List<CommandInfoModel>() { command });

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
                                CommandCode = "0x010E",
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
                                CommandCode = "0x010F",
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
                    CommandCode = "0x010D",
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
                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType.PE, InstructionType.Query, commandList.Value);

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
