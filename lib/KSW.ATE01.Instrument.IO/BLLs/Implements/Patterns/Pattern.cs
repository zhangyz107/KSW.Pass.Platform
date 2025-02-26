using KSW.ATE01.Instrument.IO.BLLs.Abstractions.Patterns;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Instruments;
using KSW.ATE01.Instrument.IO.Enums.Instruments;
using KSW.ATE01.Instrument.IO.Enums.Patterns;
using KSW.ATE01.Instrument.IO.Helpers;
using KSW.ATE01.Instrument.IO.Models.Instruments;
using KSW.ATE01.Instrument.IO.Models.Results;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Models;
using KSW.ATE01.Project.Base.Models.Patterns;
using System.IO;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.Patterns
{
    public class Pattern : PE131CommandBase<Pattern>, IPattern
    {
        private const long _mbByte = 128 * 1024 * 1024L;
        private const int _maxChannelNum = 127;
        private const int _packageAdditionalLength = 8;
        private List<PatternModel> _patterns = new List<PatternModel>();

        public List<PatternModel> Patterns { get => _patterns; }

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

            try
            {
                //  组装数据包
                var commandList = new List<CommandInfoModel>();

                foreach (var pin in PinList)
                {
                    foreach (var site in pin.Sites)
                    {
                        var channelNum = ChannelManagerHelper.GetChannelNumBySlot(site.SiteValue);
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

        public void SetPinInit(PinInitVoltageType pinInitVoltageType)
        {
            if (PinList == null || !PinList.Any())
                return;

            try
            {
                //  组装数据包
                var commandList = new List<CommandInfoModel>();

                foreach (var pin in PinList)
                {
                    foreach (var site in pin.Sites)
                    {
                        var channelNum = ChannelManagerHelper.GetChannelNumBySlot(site.SiteValue);
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

                var patternFile = Instance?._patterns.FirstOrDefault(x => x.PatternFileName.ToLower().Equals(patternName));
                if (patternFile == null)
                    return;

                var dataStartAddress = patternFile.DataStartAddress;
                var dataLength = patternFile.PinDataLength;
                var pinList = patternFile.PatternVectors.FirstOrDefault()?.Pins;

                //  组装数据包
                var commandList = new List<CommandInfoModel>();
                foreach (var pin in pinList)
                {
                    var channel = PinManagerHelper.GetPinByName(Instance.TestPlan?.Channel, pin.PinName);
                    var pinIndex = PinManagerHelper.GetPinIndexByPinName(Instance.TestPlan?.Channel, pin.PinName);
                    var dataEndAddress = dataStartAddress + dataLength;
                    foreach (var site in channel.Sites)
                    {
                        var channelNum = ChannelManagerHelper.GetChannelNumBySlot(site.SiteValue);
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
                    dataStartAddress = dataEndAddress;
                }


                if (commandList.Any())
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, BoradType.PE, InstructionType.Configuration, commandList);
                    if (Instance?.ControlService != null && Instance?.PE131 != null && message.Any())
                        Instance?.ControlService.Send(Instance?.PE131, message);
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

        private static void SendPatternPackageToInstrument(List<PatternPackageModel> packages)
        {
            try
            {
                foreach (var package in packages)
                {
                    var channel = PinManagerHelper.GetPinByName(Instance.TestPlan?.Channel, package.PinName);
                    if (channel == null)
                        continue;

                    foreach (var site in channel.Sites)
                    {
                        var channelNum = ChannelManagerHelper.GetChannelNumBySlot(site.SiteValue);
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
                                CommandCode = "0x010C",
                                CommandContent = contentBytes.ToArray(),
                            };

                            var message = CommandHelper.GetCommandBytes(0xFF, BoradType.PE, InstructionType.Configuration, new List<CommandInfoModel>() { command });
                            if (Instance?.ControlService != null && Instance?.PE131 != null && message.Any())
                                Instance?.ControlService.Send(Instance?.PE131, message);
                        }

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

#if DEBUG
        public void SetPatternEnable(bool enable)
        {
            if (PinList == null || !PinList.Any())
                return;

            try
            {
                int maxChannelNum = 0;
                //  组装数据包
                var commandList = new List<CommandInfoModel>();
                var channelList = new int[] { 31, 63, 95, 127 };
                var enableByte = System.Convert.ToByte(enable);
                foreach (var channel in channelList)
                {
                    var channelByte = System.Convert.ToByte(channel);

                    var command = new CommandInfoModel()
                    {
                        CommandCode = "0xfeff",
                        CommandContent = new byte[] { channelByte, enableByte },
                    };
                    commandList.Add(command);
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

        public ChannelResultModel<bool> GetPatternEnable()
        {
            ChannelResultModel<bool> result = null;
            if (PinList == null || !PinList.Any())
                return result;

            try
            {
                int maxChannelNum = 0;
                //  组装数据包
                var commandList = new List<CommandInfoModel>();
                foreach (var pin in PinList)
                {
                    foreach (var site in pin.Sites)
                    {
                        var channelNum = ChannelManagerHelper.GetChannelNumBySlot(site.SiteValue);
                        if (channelNum >= 0 && maxChannelNum < channelNum)
                        {
                            maxChannelNum = channelNum;
                        }
                    }
                }

                var channelByte = System.Convert.ToByte(maxChannelNum);

                commandList.Add(new CommandInfoModel()
                {
                    CommandCode = "0xfeff",
                    CommandContent = new byte[] { channelByte },
                });

                if (commandList.Any())
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, BoradType.PE, InstructionType.Query, commandList);
                    if (ControlService != null && message.Any())
                    {
                        var queryResult = ControlService.Query(PE131, message);
                        var commands = CommandHelper.ConversionBytesToCommands(queryResult);

                        foreach (var command in commands)
                        {
                            if (command.CommnadLength >= 2)
                            {
                                result = new ChannelResultModel<bool>();
                                result.ChannelNum = (int)command.CommandContent[0];
                                result.OriginalData = command.CommandContent;
                                if (result.ChannelNum >= 0)
                                {
                                    result.Site = ChannelManagerHelper.GetSlotByChannelNum(result.ChannelNum);
                                    result.PinName = PinManagerHelper.GetPinNameBySlotName(Instance.TestPlan?.Channel, result.Site);
                                }
                                result.SiteResult = BitConverter.ToBoolean(command.CommandContent, 1);
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
#endif
        public List<PatternRunningStateModel> GetRunningState()
        {
            if (PinList == null || !PinList.Any())
                return null;

            var allChannelResult = GetAllChannelRunningState();
            var pinNames = PinList.Select(x => x.PinName);

            return allChannelResult.Where(x => pinNames.Contains(x.PinName)).ToList();
        }

        public List<ChannelResultModel<int>> GetFailPosition()
        {
            var result = new List<ChannelResultModel<int>>();
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

                if (commandList.Any())
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, BoradType.PE, InstructionType.Query, commandList);
                    if (ControlService != null && message.Any())
                    {
                        var queryResult = ControlService.Query(PE131, message);
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
                                    tempData.Site = ChannelManagerHelper.GetSlotByChannelNum(tempData.ChannelNum);
                                    tempData.PinName = PinManagerHelper.GetPinNameBySlotName(Instance.TestPlan?.Channel, tempData.Site);
                                }
                                tempData.SiteResult = BitConverter.ToInt32(command.CommandContent.AsSpan(1, 4).ToArray().Reverse().ToArray());  //存在疑问，未调试
                                result.Add(tempData);
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
                var commandList = new List<CommandInfoModel>();
                var addressBytes = BitConverter.GetBytes(startAddress).Reverse().ToArray();
                var lengthBytes = BitConverter.GetBytes(length).Reverse().ToArray();

                foreach (var pin in PinList)
                {
                    foreach (var site in pin.Sites)
                    {
                        var channelNum = ChannelManagerHelper.GetChannelNumBySlot(site.SiteValue);
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

                if (commandList.Any())
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, BoradType.PE, InstructionType.Query, commandList);
                    if (ControlService != null && message.Any())
                    {
                        var queryResult = ControlService.Query(PE131, message);
                        var commands = CommandHelper.ConversionBytesToCommands(queryResult);

                        foreach (var command in commands)
                        {
                            if (command.CommnadLength >= length + 5)
                            {
                                var tempData = ContentToChannelResult(command.CommandContent);
                                result.Add(tempData);
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

        private ChannelResultModel<PatternResultModel> ContentToChannelResult(byte[] commandContent)
        {
            var result = new ChannelResultModel<PatternResultModel>();

            try
            {
                result.ChannelNum = (int)commandContent[0];
                result.OriginalData = commandContent;
                if (result.ChannelNum >= 0)
                {
                    result.Site = ChannelManagerHelper.GetSlotByChannelNum(result.ChannelNum);
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

        public static List<PatternRunningStateModel> GetAllChannelRunningState()
        {
            var result = new List<PatternRunningStateModel>();

            try
            {
                var sendCommand = new CommandInfoModel()
                {
                    CommandCode = "0x010D",
                    CommandContent = new byte[] { 255 }
                };

                var message = CommandHelper.GetCommandBytes(0xFF, BoradType.PE, InstructionType.Query, new List<CommandInfoModel>() { sendCommand });
                if (Instance.ControlService != null && message.Any())
                {
                    var queryResult = Instance.ControlService.Query(Instance.PE131, message);
                    var commands = CommandHelper.ConversionBytesToCommands(queryResult);

                    foreach (var command in commands)
                    {
                        if (command.CommnadLength >= 33)
                        {
                            var tempData = ContentToChannelRunningState(command.CommandContent);
                            result.AddRange(tempData);
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

        private static List<PatternRunningStateModel> ContentToChannelRunningState(byte[] commandContent)
        {
            var result = new List<PatternRunningStateModel>();

            try
            {
                if (commandContent.Any() && commandContent.Length >= 33)
                {
                    var passArray = commandContent.AsSpan().Slice(1, 16).ToArray();
                    var runningArray = commandContent.AsSpan().Slice(17, 16).ToArray();

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
                            tempData.Site = ChannelManagerHelper.GetSlotByChannelNum(i);
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
                    var bit = (currentByte & (1 << (7 - j))) != 0;
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
                var commandList = new List<CommandInfoModel>();


                foreach (var pin in PinList)
                {
                    foreach (var site in pin.Sites)
                    {
                        var channelNum = ChannelManagerHelper.GetChannelNumBySlot(site.SiteValue);
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

                if (commandList.Any())
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, BoradType.PE, InstructionType.Query, commandList);
                    if (ControlService != null && message.Any())
                    {
                        var queryResult = ControlService.Query(PE131, message);
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
                                    tempData.Site = ChannelManagerHelper.GetSlotByChannelNum(tempData.ChannelNum);
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

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
