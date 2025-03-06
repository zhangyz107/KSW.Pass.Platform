using KSW.ATE01.Instrument.IO.BLLs.Abstractions.Backplanes;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Instruments;
using KSW.ATE01.Instrument.IO.Enums.Instruments;
using KSW.ATE01.Instrument.IO.Helpers;
using KSW.ATE01.Instrument.IO.Models.Instruments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Resources;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.Backplanes
{
    public class Backplane : BackplaneCommandBase<Backplane>, IBackplane
    {
        public void SetChannelEnable(bool enable, int slot, int channel)
        {
            try
            {
                if (slot > 16 || slot < 0)
                    throw new ArgumentOutOfRangeException($"slot{slot}超出范围");

                var result = new byte[8];
                var controlService = Instance?.ControlService;
                var enableByte = GetChannelEnableByte(enable, channel);

                if (enable)
                {
                    var isEven = slot % 2 == 0;
                    var index = slot / 2;
                    if (!isEven)
                        enableByte = (byte)(enableByte << 4);

                    result[index] = enableByte;
                }
                var command = new CommandInfoModel()
                {
                    CommandCode = "0x0000",
                    CommandContent = result,
                };

                var message = CommandHelper.GetCommandBytes(0xFE, BoardType, InstructionType.Configuration, new List<CommandInfoModel>() { command });
                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(BoardType, "0xFE");
                if (controlService != null && instrumentInfo != null)
                    controlService.Send(instrumentInfo, message);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public void SetEnable(bool enable, int slot, int channel = -1)
        {
            try
            {
                if (slot > 16 || slot < 0)
                    throw new ArgumentOutOfRangeException($"slot{slot}超出范围");

                var controlService = Instance?.ControlService;
                var result = new byte[8];
                byte enableByte = 0;
                if (enable)
                {
                    enableByte = 15;

                    var isEven = slot % 2 == 0;
                    var index = slot / 2;
                    if (!isEven)
                        enableByte = (byte)(enableByte << 4);
                    result[index] = enableByte;
                }

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x0000",
                    CommandContent = result,
                };

                var message = CommandHelper.GetCommandBytes(0xFE, BoardType, InstructionType.Configuration, new List<CommandInfoModel>() { command });
                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(BoardType, "0xFE");
                if (controlService != null && instrumentInfo != null)
                    controlService.Send(instrumentInfo, message);

            }
            catch (Exception)
            {

                throw;
            }
        }

        private byte GetChannelEnableByte(bool enable, int channel)
        {
            byte enableByte = 0;

            if (enable)
                return enableByte;

            if (channel <= 31)
                enableByte = 1;
            else if (channel <= 63)
                enableByte = 2;
            else if (channel <= 95)
                enableByte = 4;
            else if (channel <= 127)
                enableByte = 8;

            return enableByte;
        }
    }
}
