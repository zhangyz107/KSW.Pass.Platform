using KSW.ATE01.Pattern.Domain.Projects.Core.Enums;
using KSW.Dtos;

namespace KSW.ATE01.Pattern.Application.Models.Projects
{
    /// <summary>
    /// 命令信息
    /// </summary>
    public struct CommandModel
    {
        /// <summary>
        /// 类型
        /// </summary>
        public CommandType Type;

        public string TypeDescription => Type == CommandType.nop ? string.Empty : Type.Description();

        /// <summary>
        /// 指令参数
        /// </summary>
        public object CommandParameter;

        /// <summary>
        /// 命令全内容
        /// </summary>
        public string CommandFullContent;

    }
}
