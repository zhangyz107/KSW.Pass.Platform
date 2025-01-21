using KSW.Application;
using KSW.ATE01.Pattern.Application.BLLs.Abstractions.Instruments;
using KSW.ATE01.Pattern.Application.Extensions;
using KSW.ATE01.Pattern.Application.Models.Instruments;
using KSW.ATE01.Pattern.Domain.Instruments.Core.Enums;

namespace KSW.ATE01.Pattern.Application.BLLs.Implements.Instruments
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandBLL : ServiceBase, ICommandBLL
    {
        private readonly string _messageHeader = "0xBEAD";
        private readonly string _messageEnd = "0xDEAD";

        public CommandBLL(
            IContainerProvider containerProvider) : base(containerProvider)
        {

        }

        public byte[] GetCommandBytes(byte slotNum, BoradType boradType, InstructionType instructionType, List<CommandInfoModel> commands)
        {
            if (commands.IsEmpty())
                return null;

            var message = new List<byte>();
            //帧头
            message.AddRange(_messageHeader.ToByteArray());

            var realCommands = commands.Where(x => x != null && !x.CommandContent.IsEmpty());
            var commandsLength = realCommands.Sum(x => x.CommnadLength) + realCommands.Count() * 4;
            var totalLength = 2 + 2 + 1 + 2 + 1 + 2 + commandsLength + 2;

            //帧长
            var frameLength = BitConverter.GetBytes((short)totalLength).Reverse();
            message.AddRange(frameLength);

            //槽位号
            message.Add(slotNum);

            //板卡类型
            message.AddRange(boradType.Description().ToByteArray());

            //指令类型
            message.AddRange(instructionType.Description().ToByteArray());

            //指令数量
            var commandCount = BitConverter.GetBytes((short)commands.Where(x => x != null && !x.CommandContent.IsEmpty()).Count()).Reverse();
            message.AddRange(commandCount);

            foreach (var command in commands)
            {
                if (command.CommnadLength > 0)
                {
                    //ID
                    message.AddRange(command.CommandCode.ToByteArray());
                    //指令长度
                    var commandLength = BitConverter.GetBytes((short)(command.CommnadLength + 4)).Reverse();
                    message.AddRange(commandLength);

                    //指令内容
                    message.AddRange(command.CommandContent);
                }
            }

            //帧尾
            message.AddRange(_messageEnd.ToByteArray());

            return message.ToArray();
        }
    }
}
