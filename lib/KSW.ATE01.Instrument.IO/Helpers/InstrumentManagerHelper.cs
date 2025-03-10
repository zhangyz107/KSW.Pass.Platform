using KSW.ATE01.Instrument.IO.BLLs.Implements;
using KSW.ATE01.Instrument.IO.Enums.Instruments;
using KSW.ATE01.Instrument.IO.Models.Instruments;
using KSW.ATE01.Project.Base.Helpers;

namespace KSW.ATE01.Instrument.IO.Helpers
{
    public class InstrumentManagerHelper
    {
        private int _maxSlotNum = 16;   //最大插槽数
        private readonly IInstrumentControlFactory _controlFactory;
        private readonly Dictionary<BoardInfoModel, InstrumentBaseModel> _instrumentDic = new Dictionary<BoardInfoModel, InstrumentBaseModel>();
        private static readonly Lazy<InstrumentManagerHelper> _instance = new Lazy<InstrumentManagerHelper>(() => new InstrumentManagerHelper());

        public InstrumentManagerHelper()
        {
            _controlFactory = new InstrumentControlFactory();
            _controlFactory.RegisterControls();

            // 添加背板信息
            InitInstrumentDic();

            // 循环查询已使用插槽
            QuerySlotInfo();
        }

        private void InitInstrumentDic()
        {
            _instrumentDic.Add(new BoardInfoModel()
            {
                BoardName = "Backplane",
                BoardType = BoardType.Backplane,
                SlotNum = "0xFE"
            },
            new UdpInstrumentModel()
            {
                InstrumentName = "Backplane",
                IpAddress = "192.168.0.216",
                Port = 40288,
                LocalPort = 9988,
                ConnectType = IOTypeEnum.UDP,
            });
        }

        private void QuerySlotInfo(IOTypeEnum ioType = IOTypeEnum.UDP)
        {
            var control = _controlFactory.GetInstrumentControlService(ioType);
            var port = 40288;
            var commandInfo = new CommandInfoModel()
            {
                CommandCode = "0xFF00",
                CommandContent = Array.Empty<byte>(),
            };
            var message = CommandHelper.GetCommandBytes(0xFF, BoardType.Unknown, InstructionType.Query, new List<CommandInfoModel>() { commandInfo });
            for (int i = 0; i < _maxSlotNum; i++)
            {
                var endIp = 200 + i;
                var ipAddress = $"192.168.0.{endIp}";
                if (control.TestConnect(ipAddress))
                {
                    var queryResult = control.Query(ipAddress, port, message, out int localPort);
                    var commands = CommandHelper.ConversionBytesToCommands(queryResult);

                    if (commands != null && commands.Any())
                    {
                        var command = commands.FirstOrDefault();
                        if (command.CommandContent.Length >= 3)
                        {
                            var slot = command.CommandContent[0];
                            var type = BitConverter.ToInt16(command.CommandContent, 1);

                            var boardType = (BoardType)type;
                            var boardInfos = _instrumentDic.Keys.Where(x => x.BoardType == boardType);

                            _instrumentDic.Add(new BoardInfoModel()
                            {
                                BoardName = boardType.GetDescription(),
                                BoardType = boardType,
                                SlotNum = $"0x{slot.ToString("x2")}"
                            },
                            new UdpInstrumentModel()
                            {
                                InstrumentName = $"{boardType.GetDescription()}-{boardInfos.Count() + 1}",
                                IpAddress = ipAddress,
                                Port = 40288,
                                LocalPort = localPort,
                                ConnectType = IOTypeEnum.UDP,
                            });
                        }
                    }
                }
            }
        }

        public static List<InstrumentBaseModel> GetInstrumentInfosByBoardType(BoardType boardType, string slot = null)
        {
            List<InstrumentBaseModel> result = new List<InstrumentBaseModel>();
            var helper = _instance.Value;
            if (helper == null)
                return result;

            var boardInfos = helper._instrumentDic.Keys.Where(x => x.BoardType == boardType);
            if (!string.IsNullOrEmpty(slot))
                boardInfos = boardInfos.Where(x => x.SlotNum.ToLower().Equals(slot.ToLower()));

            foreach (var boardInfo in boardInfos)
            {
                if (helper._instrumentDic.ContainsKey(boardInfo))
                {
                    var instrumentModel = helper._instrumentDic[boardInfo];
                    result.Add(instrumentModel);
                }
            }

            return result;
        }

        public static IInstruentControlService GetControlServiceByBoardType(BoardType boardType)
        {
            var helper = _instance.Value;
            if (helper == null)
                return null;

            var instrumentInfo = GetInstrumentInfosByBoardType(boardType)?.FirstOrDefault();
            if (instrumentInfo == null)
                return null;

            return helper._controlFactory.GetInstrumentControlService(instrumentInfo?.ConnectType);
        }

        public static InstrumentBaseModel GetInstrumentInfoByBoardType(BoardType boardType, string slot)
        {
            return GetInstrumentInfosByBoardType(boardType, slot)?.FirstOrDefault();
        }
    }
}
