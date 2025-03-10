using KSW.ATE01.Instrument.IO.BLLs.Abstractions.Backplanes;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Instruments;
using KSW.ATE01.Instrument.IO.Enums.Instruments;
using KSW.ATE01.Instrument.IO.Helpers;
using KSW.ATE01.Instrument.IO.Models.Instruments;
using KSW.ATE01.Project.Base.Models;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.Backplanes
{
    public class Backplane : BackplaneCommandBase<Backplane>, IBackplane
    {
        public static IBackplane Pins(string pinList)
        {
            if (string.IsNullOrEmpty(pinList))
                throw new ArgumentNullException(nameof(pinList));

            Instance.GetPinList(pinList);

            return Instance;
        }

        public void SetChannelEnable(bool enable)
        {
            if (PinList == null || !PinList.Any())
                return;

            var result = new byte[8];
            var controlService = Instance?.ControlService;
            Dictionary<int, List<int>> slotChannelDic = new Dictionary<int, List<int>>();

            try
            {
                foreach (var pin in PinList)
                {
                    foreach (var site in pin.Sites)
                    {
                        if (!ChannelManagerHelper.IsSiteValid(site.SiteName))
                            continue;

                        var channelNum = ChannelManagerHelper.GetChannelNumSiteInfo(site.SiteValue, out int slot);
                        if (slot > 16 || slot < 0)
                            throw new ArgumentOutOfRangeException($"{site.SiteValue}的slot{slot}超出范围");

                        var channelList = new List<int>();
                        if (!slotChannelDic.ContainsKey(slot))
                            slotChannelDic.Add(slot, channelList);
                        else
                            channelList = slotChannelDic[slot];

                        if (!channelList.Any(x => x == channelNum))
                            channelList.Add(channelNum);
                    }
                }

                foreach (var slotChannel in slotChannelDic)
                {
                    var slot = slotChannel.Key;
                    var enableByte = GetChannelListEnableByte(slotChannel.Value);
                    var isEven = slot % 2 == 0;
                    var index = slot / 2;
                    if (!isEven)
                        enableByte = (byte)(enableByte << 4);
                    result[index] = enableByte;

                }

                var command = new CommandInfoModel();
                command.CommandContent = result.Reverse().ToArray();

                if (enable)
                    command.CommandCode = "0x0001";
                else
                    command.CommandCode = "0x0002";

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

        public static void SetEnable(bool enable, int slot)
        {
            try
            {
                if (slot > 16 || slot < 0)
                    throw new ArgumentOutOfRangeException($"slot{slot}超出范围");

                var controlService = Instance?.ControlService;
                var result = new byte[8];
                byte enableByte = 15;

                var isEven = slot % 2 == 0;
                var index = slot / 2;
                if (!isEven)
                    enableByte = (byte)(enableByte << 4);
                result[index] = enableByte;

                var command = new CommandInfoModel();
                command.CommandContent = result.Reverse().ToArray();

                if (enable)
                    command.CommandCode = "0x0001";
                else
                    command.CommandCode = "0x0002";

                var message = CommandHelper.GetCommandBytes(0xFE, (BoardType)Instance?.BoardType, InstructionType.Configuration, new List<CommandInfoModel>() { command });
                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType((BoardType)Instance?.BoardType, "0xFE");
                if (controlService != null && instrumentInfo != null)
                    controlService.Send(instrumentInfo, message);

            }
            catch (Exception)
            {

                throw;
            }
        }

        private byte GetChannelListEnableByte(List<int> channels)
        {
            byte enableByte = 0;

            foreach (int channel in channels)
            {
                if (channel <= 31)
                    enableByte |= 1;
                else if (channel <= 63)
                    enableByte |= 2;
                else if (channel <= 95)
                    enableByte |= 4;
                else if (channel <= 127)
                    enableByte |= 8;
            }

            return enableByte;
        }
    }
}
