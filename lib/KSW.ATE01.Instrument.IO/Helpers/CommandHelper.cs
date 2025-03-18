/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：CommandHelper.cs
// 功能描述：控制指令帮助类
//
// 作者：zhangyingzhong
// 日期：2025/01/21 16:53
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.ATE01.Instrument.IO.Enums.Instruments;
using KSW.ATE01.Instrument.IO.Models.Instruments;
using KSW.ATE01.Project.Base.Extensions;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Models.Errors;
using System.Runtime.InteropServices;

namespace KSW.ATE01.Instrument.IO.Helpers
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct FrameStruct
    {
        short header;           //帧头
        short frameLength;      //帧长
        byte slotNumber;        //槽位号
        short boardType;        //板卡类型
        byte instructionType;   //指令类型
        short instructionCount; //指令数量
        short tail;             //帧尾
    }

    /// <summary>
    /// 控制指令帮助类
    /// </summary>
    public class CommandHelper
    {
        private const string _messageHeader = "0xBEAD";
        private const string _messageEnd = "0xDEAD";
        private static FrameStruct _frameStruct = default;

        public static byte[] GetCommandBytes(byte slotNum, BoardType boradType, InstructionType instructionType, List<CommandInfoModel> commands)
        {
            if (!commands.Any())
                return null;

            var message = new List<byte>();

            //帧头
            message.AddRange(_messageHeader.ToByteArray());
            var realCommands = commands.Where(x => x != null && !string.IsNullOrEmpty(x.CommandCode));
            var commandsLength = realCommands.Sum(x => x.CommnadLength) + realCommands.Count() * 4;
            var totalLength = Marshal.SizeOf(_frameStruct) + commandsLength;
            //帧长
            var frameLength = BitConverter.GetBytes((short)totalLength).Reverse();
            message.AddRange(frameLength);

            //槽位号
            message.Add(slotNum);

            //板卡类型
            message.AddRange(((int)boradType).ToString("x4").ToByteArray());

            //指令类型
            message.AddRange(instructionType.GetDescription().ToByteArray());

            //指令数量
            var commandCount = BitConverter.GetBytes((short)commands.Where(x => x != null && !string.IsNullOrEmpty(x.CommandCode)).Count()).Reverse();
            message.AddRange(commandCount);

            foreach (var command in commands)
            {
                if (command.CommnadLength >= 0)
                {
                    //ID
                    message.AddRange(command.CommandCode.ToByteArray());
                    //指令长度
                    var commandLength = BitConverter.GetBytes((short)(command.CommnadLength + 4)).Reverse();
                    message.AddRange(commandLength);

                    //指令内容
                    if (command.CommandContent != null && command.CommandContent.Any())
                        message.AddRange(command.CommandContent);
                }
            }

            //帧尾
            message.AddRange(_messageEnd.ToByteArray());

            return message.ToArray();
        }

        public static List<CommandInfoModel> ConversionBytesToCommands(byte[] data)
        {

            var result = new List<CommandInfoModel>();
            if (!data.Any())
                return result;

            var location = nameof(ConversionBytesToCommands);

            try
            {
                var type = Enum.ToObject(typeof(InstructionType), data[7]) as InstructionType?;
                if (type == null)
                    ErrorMessages.IO.IOResultAbnormal();
                else if (type == InstructionType.ConfigurationFailed)
                    ErrorMessages.IO.IOConfigurationFailed();
                else if (type == InstructionType.QueryFailed)
                    ErrorMessages.IO.IOInvalidQuery();

                var slotNum = data[4];
                var startIndex = 8;

                var span = data.AsSpan();

                var commandCountBytes = span.Slice(startIndex, 2).ToArray().Reverse().ToArray();
                var commandCount = BitConverter.ToInt16(commandCountBytes);
                startIndex += 2;

                for (var i = 0; i < commandCount; i++)
                {
                    var commandCodeBytes = span.Slice(startIndex, 2).ToArray();
                    var commandCode = $"0x{BitConverter.ToString(commandCodeBytes).Replace("-", "")}";
                    startIndex += 2;

                    var commandLengthBytes = span.Slice(startIndex, 2).ToArray().Reverse().ToArray();
                    var commandLength = BitConverter.ToInt16(commandLengthBytes);
                    startIndex += 2;

                    var contentLength = commandLength - 4;
                    var contentBytes = span.Slice(startIndex, contentLength).ToArray();
                    startIndex += contentLength;
                    var command = new CommandInfoModel()
                    {
                        SlotNum = slotNum,
                        CommandCode = commandCode,
                        CommandContent = contentBytes,
                    };

                    result.Add(command);
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
