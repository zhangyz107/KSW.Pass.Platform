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

        public static byte[] GetCommandBytes(byte slotNum, BoradType boradType, InstructionType instructionType, List<CommandInfoModel> commands)
        {
            if (!commands.Any())
                return null;

            var message = new List<byte>();

            //帧头
            message.AddRange(_messageHeader.ToByteArray());
            var realCommands = commands.Where(x => x != null && x.CommandContent.Any());
            var commandsLength = realCommands.Sum(x => x.CommnadLength) + realCommands.Count() * 4;
            var totalLength = Marshal.SizeOf(_frameStruct) + commandsLength;
            //帧长
            var frameLength = BitConverter.GetBytes((short)totalLength).Reverse();
            message.AddRange(frameLength);

            //槽位号
            message.Add(slotNum);

            //板卡类型
            message.AddRange(boradType.GetDescription().ToByteArray());

            //指令类型
            message.AddRange(instructionType.GetDescription().ToByteArray());

            //指令数量
            var commandCount = BitConverter.GetBytes((short)commands.Where(x => x != null && x.CommandContent.Any()).Count()).Reverse();
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
