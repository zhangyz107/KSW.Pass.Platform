using KSW.ATE01.Project.Base.Enums.Patterns;
using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Models.Patterns
{
    /// <summary>
    /// 命令信息
    /// </summary>
    public class CommandModel : DtoBase
    {
        private CommandType _type;
        private object _commandParameter;
        private string _commandFullContent;

        /// <summary>
        /// 类型
        /// </summary>
        public CommandType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public string TypeDesciption => Type == CommandType.nop ? string.Empty : Type.Description();

        /// <summary>
        /// 指令参数
        /// </summary>
        public object CommandParameter
        {
            get => _commandParameter;
            set => SetProperty(ref _commandParameter, value);
        }

        /// <summary>
        /// 命令全文
        /// </summary>
        public string CommandFullContent
        {
            get => _commandFullContent;
            set => SetProperty(ref _commandFullContent, value);
        }

    }
}
