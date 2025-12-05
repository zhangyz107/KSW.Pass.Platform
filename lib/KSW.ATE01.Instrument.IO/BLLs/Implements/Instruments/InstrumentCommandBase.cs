using KSW.ATE01.Instrument.IO.Enums.Instruments;
using KSW.ATE01.Instrument.IO.Helpers;
using KSW.ATE01.Instrument.IO.Models.Instruments;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Models;
using KSW.ATE01.Project.Base.Models.TestPlans;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.Instruments
{
    public abstract class InstrumentCommandBase<T> where T : new()
    {
        protected static readonly Lazy<T> _instance = new Lazy<T>((() => new T()));

        protected TestPlanModel TestPlan { get => CommonData.Instance?.TestPlan; }

        protected List<ChannelModel> PinList { get; private set; }

        protected string CommonPinList { get; set; }

        protected virtual BoardType BoardType { get; }

        internal static T Instance { get => _instance.Value; }

        protected virtual IInstruentControlService ControlService { get => InstrumentManagerHelper.GetControlServiceByBoardType(BoardType); }

        protected virtual List<ChannelModel> GetPinList(string pinList)
        {
            if (string.IsNullOrEmpty(pinList))
                return null;

            PinList = PinManagerHelper.GetPinsByNameOrGroupName(TestPlan?.Channel, pinList);
            return PinList;
        }

        public virtual void SetMasterTriggerParam(bool isBackPlane, int tiggerNum, uint delay, bool isDebug = false)
        {
            var pins = PinManagerHelper.GetPinsByNameOrGroupName(TestPlan?.Channel, CommonPinList);

            var commandListDic = new Dictionary<int, List<CommandInfoModel>>();
            var delayArray = BitConverter.GetBytes(delay);

            try
            {
                foreach (var pin in pins)
                {
                    foreach (var site in pin.Sites)
                    {
                        if (!ChannelManagerHelper.IsSiteValid(site.SiteName))
                            continue;

                        var channelNum = ChannelManagerHelper.GetChannelNumSiteInfo(site.SiteValue, out int slot);
                        var fpga = GetFPGANum(channelNum);

                        var commandList = new List<CommandInfoModel>();
                        if (!commandListDic.ContainsKey(slot))
                            commandListDic[slot] = commandList;
                        else
                            commandList = commandListDic[slot];

                        if (channelNum >= 0)
                        {
                            var byteList = new List<byte>();
                            byteList.Add((byte)fpga);
                            byteList.Add(isBackPlane ? (byte)0x01 : (byte)0x00);
                            byteList.Add(isDebug ? (byte)0x01 : (byte)0x00);
                            byteList.Add((byte)tiggerNum);
                            byteList.AddRange(delayArray.AsSpan(0, 3).ToArray());

                            var command = new CommandInfoModel()
                            {
                                CommandCode = "0xFF02",
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
                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType, InstructionType.Configuration, commandList.Value);
                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(BoardType, slotNum);


                        if (ControlService != null && instrumentInfo != null)
                            ControlService.Send(instrumentInfo, message);
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public virtual void SetMasterTriggerEnable(bool enable)
        {
            var pins = PinManagerHelper.GetPinsByNameOrGroupName(TestPlan?.Channel, CommonPinList);

            var commandListDic = new Dictionary<int, List<CommandInfoModel>>();

            try
            {
                foreach (var pin in pins)
                {
                    foreach (var site in pin.Sites)
                    {
                        if (!ChannelManagerHelper.IsSiteValid(site.SiteName))
                            continue;

                        var channelNum = ChannelManagerHelper.GetChannelNumSiteInfo(site.SiteValue, out int slot);
                        var fpga = GetFPGANum(channelNum);
                        var commandList = new List<CommandInfoModel>();
                        if (!commandListDic.ContainsKey(slot))
                            commandListDic[slot] = commandList;
                        else
                            commandList = commandListDic[slot];

                        if (channelNum >= 0)
                        {
                            var command = new CommandInfoModel()
                            {
                                CommandCode = "0xFF03",
                                CommandContent = new byte[]
                                {
                                    (byte)fpga,
                                    enable ? (byte)0x01 : (byte)0x00
                                }
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
                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType, InstructionType.Configuration, commandList.Value);

                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(BoardType, slotNum);

                        if (ControlService != null && instrumentInfo != null)
                            ControlService.Send(instrumentInfo, message);
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private int GetFPGANum(int channelNum)
        {
            int fpga = 0;

            if (BoardType == BoardType.PE)
            {
                if (channelNum < 32)
                {
                    fpga = 0;
                }
                else if (channelNum < 64)
                {
                    fpga = 1;
                }
                else if (channelNum < 96)
                {
                    fpga = 2;
                }
                else if (channelNum < 128)
                {
                    fpga = 3;
                }
            }
            else if (BoardType == BoardType.DPS)
            {
                fpga = 0;
            }

            return fpga;
        }

        public virtual void SetSlaveTriggerParam(int tiggerNum, uint delay)
        {
            var pins = PinManagerHelper.GetPinsByNameOrGroupName(TestPlan?.Channel, CommonPinList);

            var commandListDic = new Dictionary<int, List<CommandInfoModel>>();
            var delayArray = BitConverter.GetBytes(delay);

            try
            {
                foreach (var pin in pins)
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
                            byteList.Add((byte)tiggerNum);
                            byteList.AddRange(delayArray.AsSpan(0, 3).ToArray());
                            var command = new CommandInfoModel()
                            {
                                CommandCode = "0xFF04",
                                CommandContent = byteList.ToArray()
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
                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType, InstructionType.Configuration, commandList.Value);
                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(BoardType, slotNum);

                        if (ControlService != null && instrumentInfo != null)
                            ControlService.Send(instrumentInfo, message);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public virtual void SetSlaveTriggerEnable(bool enable)
        {
            var pins = PinManagerHelper.GetPinsByNameOrGroupName(TestPlan?.Channel, CommonPinList);

            var commandListDic = new Dictionary<int, List<CommandInfoModel>>();

            try
            {
                foreach (var pin in pins)
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

                            if (channelNum >= 0)
                            {
                                var command = new CommandInfoModel()
                                {
                                    CommandCode = "0xFF05",
                                    CommandContent = new byte[]
                                    {
                                    (byte)channelNum,
                                    enable ? (byte)0x01 : (byte)0x00
                                    }
                                };
                                commandList.Add(command);
                            }
                        }
                    }
                }

                if (commandListDic.Any())
                {
                    foreach (var commandList in commandListDic)
                    {
                        var slotNum = $"0x{commandList.Key.ToString("x2")}";
                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType, InstructionType.Configuration, commandList.Value);
                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(BoardType, slotNum);

                        if (ControlService != null && instrumentInfo != null)
                            ControlService.Send(instrumentInfo, message);
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

