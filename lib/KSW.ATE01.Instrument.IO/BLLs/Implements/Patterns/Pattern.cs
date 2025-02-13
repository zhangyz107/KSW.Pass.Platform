using KSW.ATE01.Instrument.IO.BLLs.Abstractions.Patterns;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Instruments;
using KSW.ATE01.Instrument.IO.Enums.Instruments;
using KSW.ATE01.Instrument.IO.Enums.Patterns;
using KSW.ATE01.Instrument.IO.Helpers;
using KSW.ATE01.Instrument.IO.Models.Instruments;
using KSW.ATE01.Project.Base.Helpers;
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
                    var index = PinList.IndexOf(pin);
                    var byteList = new List<byte>();
                    byteList.Add((byte)index);        //暂时按顺序下发通道（后续需要映射站点信息）
                    byteList.Add(System.Convert.ToByte(pinIOType));    //Type

                    var command = new CommandInfoModel()
                    {
                        CommandCode = "0x0108",
                        CommandContent = byteList.ToArray(),
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
                    var index = PinList.IndexOf(pin);
                    var byteList = new List<byte>();
                    byteList.Add((byte)index);              //暂时按顺序下发通道（后续需要映射站点信息）
                    byteList.Add(System.Convert.ToByte(pinInitVoltageType)); //Type

                    var command = new CommandInfoModel()
                    {
                        CommandCode = "0x0109",
                        CommandContent = byteList.ToArray(),
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

        public void SetPatternParam()
        {
            if (PinList == null || !PinList.Any())
                return;

            try
            {
                //  组装数据包
                var commandList = new List<CommandInfoModel>();
                foreach (var pin in PinList)
                {
                    var index = PinList.IndexOf(pin);
                    var patternStartAddress = index * _mbByte;
                    var patternEndAddress = (index + 1) * _mbByte;
                    var channelGroup = index / _maxChannelNum + 1;
                    var receiveStartAddress = (channelGroup * _maxChannelNum + index) * _mbByte;
                    var receiveEndAddress = (channelGroup * _maxChannelNum + index + 1) * _mbByte;
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
                    byteList.Add((byte)index);
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

        public static void SetPatternFile(string[] patternFiles)
        {
            if (patternFiles == null || !patternFiles.Any())
                return;

            try
            {
                Instance?._patterns?.Clear();
                foreach (string patternFile in patternFiles)
                {
                    if (!File.Exists(patternFile)) continue;
                    var pattern = PatternHelper.AnalysisPattern(patternFile);
                    Instance?._patterns.Add(pattern);
                    if (pattern != null)
                    {
                        var package = PatternHelper.ConversionPatternModel(pattern);
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
                    var contentBytes = new byte[package.Length + _packageAdditionalLength];
                    contentBytes[0] = (byte)package.ChannelNum;
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
            catch (Exception)
            {

                throw;
            }
        }
    }
}
