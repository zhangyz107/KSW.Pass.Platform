using KSW.Application;
using KSW.ATE01.Pattern.Application.Models.Instruments;
using KSW.ATE01.Pattern.Domain.Instruments.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application.BLLs.Abstractions.Instruments
{
    /// <summary>
    /// 控制指令业务逻辑层接口
    /// </summary>
    public interface ICommandBLL : IService
    {
        /// <summary>
        /// 获取遥控命令
        /// </summary>
        /// <returns></returns>
        byte[] GetCommandBytes(byte slotNum, BoradType boradType, InstructionType instructionType, List<CommandInfoModel> commands);
    }
}
