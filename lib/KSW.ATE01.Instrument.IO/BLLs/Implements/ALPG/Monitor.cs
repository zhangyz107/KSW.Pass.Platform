using KSW.ATE01.Instrument.IO.BLLs.Abstractions.ALPG;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Instruments;
using KSW.ATE01.Instrument.IO.Enums.Instruments;
using KSW.ATE01.Instrument.IO.Enums.Monitor;
using KSW.ATE01.Instrument.IO.Helpers;
using KSW.ATE01.Instrument.IO.Models.Instruments;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Models.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.ALPG
{
    public class Monitor : PE131CommandBase<Monitor>, IMonitor
    {
        private int _alpgNum;

        public static IMonitor Num(int alpgNum)
        {
            if (alpgNum < 0 || alpgNum > 3)
                ErrorMessages.ALPG.ALPGNumOutOfRange();

            Instance._alpgNum = alpgNum;
            return Instance;
        }

        public void StartMonitor(MonitorObject monitorObject)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.Add((byte)monitorObject);

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x0061",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public byte[] GetMonitorData(ushort address, ushort length)
        {
            byte[] result = null;
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            BitConverterHelper.IsLittleEndian = false;
            byteList.AddRange(BitConverterHelper.GetBytes(address));
            byteList.AddRange(BitConverterHelper.GetBytes(length));

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x0062",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        result = controlService.Query(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

    }
}
