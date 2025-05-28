using KSW.ATE01.Instrument.IO.BLLs.Implements.Instruments;
using KSW.ATE01.Instrument.IO.Enums.Instruments;
using KSW.ATE01.Instrument.IO.Helpers;
using KSW.ATE01.Instrument.IO.Models.Instruments;
using KSW.ATE01.Instrument.IO.Models.Results;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.Calibrations
{
    public class Calibration : InstrumentCommandBase<Calibration>
    {
        /// <summary>
        /// 启动计数器
        /// </summary>
        /// <param name="boardType"></param>
        /// <param name="duration"></param>
        public static void StartCounter(BoardType boardType, int duration)
        {
            var value = BitConverter.GetBytes(duration);
            var instrumentInfos = InstrumentManagerHelper.GetInstrumentInfosByBoardType(boardType);
            var controlService = InstrumentManagerHelper.GetControlServiceByBoardType(boardType);

            var command = new CommandInfoModel()
            {
                CommandCode = "0x0130",
                CommandContent = value.Reverse().ToArray(),
            };

            var message = CommandHelper.GetCommandBytes(0xFF, boardType, InstructionType.Configuration, new List<CommandInfoModel>() { command });

            foreach (var instrumentInfo in instrumentInfos)
            {
                if (controlService != null && instrumentInfo != null)
                    controlService.Send(instrumentInfo, message);
            }
        }

        /// <summary>
        /// 获取计数器值
        /// </summary>
        /// <returns>Key：插槽号,Value：当前插槽中DPS的计数值</returns>
        public static Dictionary<string, List<CountValueModel>> GetCounterValues(BoardType boardType)
        {
            var result = new Dictionary<string, List<CountValueModel>>();
            var controlService = InstrumentManagerHelper.GetControlServiceByBoardType(boardType);
            var detailInfos = InstrumentManagerHelper.GetDetailInfosByBoardType(boardType);

            var command = new CommandInfoModel()
            {
                CommandCode = "0x0131",
            };

            var message = CommandHelper.GetCommandBytes(0xFF, boardType, InstructionType.Query, new List<CommandInfoModel>() { command });

            foreach (var detailInfo in detailInfos)
            {
                var instrumentInfo = detailInfo.Value;

                if (controlService != null && instrumentInfo != null)
                {
                    var queryResult = controlService.Query(instrumentInfo, message);
                    var resultCommands = CommandHelper.ConversionBytesToCommands(queryResult);
                    var tempData = new List<CountValueModel>();
                    if (!result.ContainsKey(detailInfo.Key.SlotNum))
                        result.Add(detailInfo.Key.SlotNum, tempData);
                    else
                        tempData = result[detailInfo.Key.SlotNum];
                    var index = 0;

                    foreach (var resultCommand in resultCommands)
                    {
                        if (resultCommand.CommandContent.Length >= command.CommnadLength)
                        {

                            for (var i = 0; i < resultCommand.CommandContent.Length; i += 4)
                            {
                                var countValue = resultCommand.CommandContent.AsSpan().Slice(i, 4).ToArray();
                                tempData.Add(new CountValueModel()
                                {
                                    Index = index++,
                                    Value = BitConverter.ToUInt32(countValue.Reverse().ToArray())
                                });
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///  PE-CALI-RO
        /// </summary>
        /// <param name="boardType"></param>
        /// <param name="roPath"></param>
        /// <param name="cdEn"></param>
        /// <param name="fdEn"></param>
        /// <param name="cdD"></param>
        /// <param name="fdD"></param>
        /// <param name="cdCa"></param>
        /// <param name="fdCa"></param>
        /// <param name="cdCb"></param>
        /// <param name="fdCb"></param>
        public static void PeCaliRo(BoardType boardType, byte roPath, byte cdEn, ushort fdEn, byte cdD, ushort fdD, byte cdCa, ushort fdCa, byte cdCb, ushort fdCb)
        {
            var instrumentInfos = InstrumentManagerHelper.GetInstrumentInfosByBoardType(boardType);
            var controlService = InstrumentManagerHelper.GetControlServiceByBoardType(boardType);

            var fdEnByteList = BitConverter.GetBytes(fdEn).Reverse().ToArray();
            var fdDByteList = BitConverter.GetBytes(fdD).Reverse().ToArray();
            var fdCaByteList = BitConverter.GetBytes(fdCa).Reverse().ToArray();
            var fdCbByteList = BitConverter.GetBytes(fdCb).Reverse().ToArray();

            var byteList = new List<byte>();
            byteList.Add(roPath);
            byteList.Add(cdEn);
            byteList.AddRange(fdEnByteList);
            byteList.Add(cdD);
            byteList.AddRange(fdDByteList);
            byteList.Add(cdCa);
            byteList.AddRange(fdCaByteList);
            byteList.Add(cdCb);
            byteList.AddRange(fdCbByteList);

            var command = new CommandInfoModel()
            {
                CommandCode = "0x0132",
                CommandContent = byteList.ToArray(),
            };

            var message = CommandHelper.GetCommandBytes(0xFF, boardType, InstructionType.Configuration, new List<CommandInfoModel>() { command });

            foreach (var instrumentInfo in instrumentInfos)
            {
                if (controlService != null && instrumentInfo != null)
                    controlService.Send(instrumentInfo, message);
            }
        }

        /// <summary>
        /// 获取校准接收数据
        /// </summary>
        /// <param name="boardType"></param>
        /// <param name="chNum"></param>
        /// <returns></returns>
        public static Dictionary<string, CalibrateReceivedData> GetCalibrateReceivedData(BoardType boardType, byte chNum)
        {
            var result = new Dictionary<string, CalibrateReceivedData>();
            var controlService = InstrumentManagerHelper.GetControlServiceByBoardType(boardType);
            var detailInfos = InstrumentManagerHelper.GetDetailInfosByBoardType(boardType);

            var command = new CommandInfoModel()
            {
                CommandCode = "0x0133",
                CommandContent = new byte[] { (byte)chNum },
            };

            var message = CommandHelper.GetCommandBytes(0xFF, boardType, InstructionType.Query, new List<CommandInfoModel>() { command });

            foreach (var detailInfo in detailInfos)
            {
                var instrumentInfo = detailInfo.Value;

                if (controlService != null && instrumentInfo != null)
                {
                    var queryResult = controlService.Query(instrumentInfo, message);
                    var resultCommands = CommandHelper.ConversionBytesToCommands(queryResult);
                    var tempData = new CalibrateReceivedData();
                    tempData.ChannelNum = chNum;
                    if (!result.ContainsKey(detailInfo.Key.SlotNum))
                        result.Add(detailInfo.Key.SlotNum, tempData);
                    else
                        tempData = result[detailInfo.Key.SlotNum];

                    var index = 1;
                    var resultCommand = resultCommands.FirstOrDefault();
                    if (resultCommand != null && resultCommand.CommandContent.Length >= command.CommnadLength)
                    {
                        tempData.CA = BitConverter.ToUInt32(resultCommand.CommandContent.AsSpan(index, 4).ToArray().Reverse().ToArray());
                        index += 4;
                        tempData.CB = BitConverter.ToUInt32(resultCommand.CommandContent.AsSpan(index, 4).ToArray().Reverse().ToArray());
                        index += 4;
                        tempData.CACount = BitConverter.ToUInt32(resultCommand.CommandContent.AsSpan(index, 4).ToArray().Reverse().ToArray());
                        index += 4;
                        tempData.CBCount = BitConverter.ToUInt32(resultCommand.CommandContent.AsSpan(index, 4).ToArray().Reverse().ToArray());
                        index += 4;
                        tempData.Position = BitConverter.ToUInt32(resultCommand.CommandContent.AsSpan(index, 4).ToArray().Reverse().ToArray());
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 切换S10固件
        /// </summary>
        /// <param name="boardType"></param>
        /// <param name="firmwareVersion"></param>
        public static void SwitchFirmware(BoardType boardType, byte firmwareVersion)
        {
            var instrumentInfos = InstrumentManagerHelper.GetInstrumentInfosByBoardType(boardType);
            var controlService = InstrumentManagerHelper.GetControlServiceByBoardType(boardType);

            var command = new CommandInfoModel()
            {
                CommandCode = "0x0134",
                CommandContent = new byte[] { firmwareVersion },
            };

            var message = CommandHelper.GetCommandBytes(0xFF, boardType, InstructionType.Configuration, new List<CommandInfoModel>() { command });

            foreach (var instrumentInfo in instrumentInfos)
            {
                if (controlService != null && instrumentInfo != null)
                    controlService.Send(instrumentInfo, message);
            }
        }
    }
}
