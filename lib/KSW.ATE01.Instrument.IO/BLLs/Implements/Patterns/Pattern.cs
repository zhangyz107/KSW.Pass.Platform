using KSW.ATE01.Instrument.IO.BLLs.Abstractions.Patterns;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Instruments;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Ppmus;
using KSW.ATE01.Instrument.IO.Enums.Instruments;
using KSW.ATE01.Instrument.IO.Enums.Patterns;
using KSW.ATE01.Instrument.IO.Helpers;
using KSW.ATE01.Instrument.IO.Models.Instruments;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Models.Patterns;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.Patterns
{
    public class Pattern : PE131CommandBase<Pattern>, IPattern
    {
        private const double _periodResolution = 6.25e-12;
        private const long _mbByte = 128 * 1024 * 1024L;
        private const int _maxChannelNum = 127;
        private const int _packageAdditionalLength = 8;

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

        public void SetTiming(double patternPeriod, WaveformType waveformType, StrobeType strobeType, double d0, double d1, double d2, double d3, double r0, double r1, sbyte pwa_en, byte cd_en, ushort fd_en, sbyte pwa_d, byte cd_d, ushort fd_d, sbyte pwa_ca, byte cd_ca, ushort fd_ca, sbyte pwa_cb, byte cd_cb, ushort fd_cb)
        {
            if (PinList == null || !PinList.Any())
                return;

            try
            {
                if (patternPeriod < 0 || patternPeriod > 0.02684354559375)
                    throw new ArgumentOutOfRangeException(nameof(patternPeriod), "取值范围:0~0.02684354559375");

                if (d0 < 0 || d0 > patternPeriod)
                    throw new ArgumentOutOfRangeException(nameof(d0), $"取值范围:0~ {patternPeriod}");

                if (d1 < 0 || d1 > patternPeriod)
                    throw new ArgumentOutOfRangeException(nameof(d1), $"取值范围:0~ {patternPeriod}");

                if (d2 < 0 || d2 > patternPeriod)
                    throw new ArgumentOutOfRangeException(nameof(d2), $"取值范围:0~ {patternPeriod}");

                if (d3 < 0 || d3 > patternPeriod)
                    throw new ArgumentOutOfRangeException(nameof(d3), $"取值范围:0~ {patternPeriod}");

                if (r0 < 0 || r0 > patternPeriod)
                    throw new ArgumentOutOfRangeException(nameof(r0), $"取值范围:0~ {patternPeriod}");

                if (r1 < 0 || r1 > patternPeriod)
                    throw new ArgumentOutOfRangeException(nameof(r1), $"取值范围:0~ {patternPeriod}");

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

                var period = System.Convert.ToUInt32(patternPeriod / _periodResolution);
                var periodBytes = BitConverter.GetBytes(period).Reverse();
                var formatByte = System.Convert.ToByte(waveformType);
                var strobeByte = System.Convert.ToByte(strobeType);
                var d0Period = System.Convert.ToUInt32(d0 / _periodResolution);
                var d0PeriodBytes = BitConverter.GetBytes(d0Period).Reverse();
                var d1Period = System.Convert.ToUInt32(d1 / _periodResolution);
                var d1PeriodBytes = BitConverter.GetBytes(d1Period).Reverse();
                var d2Period = System.Convert.ToUInt32(d2 / _periodResolution);
                var d2PeriodBytes = BitConverter.GetBytes(d2Period).Reverse();
                var d3Period = System.Convert.ToUInt32(d3 / _periodResolution);
                var d3PeriodBytes = BitConverter.GetBytes(d3Period).Reverse();
                var r0Period = System.Convert.ToUInt32(r0 / _periodResolution);
                var r0PeriodBytes = BitConverter.GetBytes(r0Period).Reverse();
                var r1Period = System.Convert.ToUInt32(r1 / _periodResolution);
                var r1PeriodBytes = BitConverter.GetBytes(r1Period).Reverse();
                var fd_enBytes = BitConverter.GetBytes(fd_en).Reverse();
                var fd_dBytes = BitConverter.GetBytes(fd_d).Reverse();
                var fd_caBytes = BitConverter.GetBytes(fd_ca).Reverse();
                var fd_cbBytes = BitConverter.GetBytes(fd_cb).Reverse();

                //  组装数据包
                var commandList = new List<CommandInfoModel>();

                foreach (var pin in PinList)
                {
                    var index = PinList.IndexOf(pin);
                    var byteList = new List<byte>();
                    byteList.Add((byte)index);              //暂时按顺序下发通道（后续需要映射站点信息）
                    byteList.AddRange(periodBytes);
                    byteList.Add(formatByte);
                    byteList.Add(strobeByte);
                    byteList.AddRange(d0PeriodBytes); //D0
                    byteList.AddRange(d1PeriodBytes); //D1
                    byteList.AddRange(d2PeriodBytes); //D2
                    byteList.AddRange(d3PeriodBytes); //D3
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
                    var command = new CommandInfoModel()
                    {
                        CommandCode = "0x010A",
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
                foreach (string patternFile in patternFiles)
                {
                    if (!File.Exists(patternFile)) continue;
                    var pattern = PatternHelper.AnalysisPattern(patternFile);
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
