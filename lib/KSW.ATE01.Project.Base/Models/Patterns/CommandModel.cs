using KSW.ATE01.Project.Base.Enums.Patterns;
using KSW.ATE01.Project.Base.Helpers;

namespace KSW.ATE01.Project.Base.Models.Patterns
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

        public string TypeDescription => Type == CommandType.nop ? string.Empty : Type.GetDescription();

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
