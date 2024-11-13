using KSW.ATE01.Pattern.Domain.Projects.Core.Enums;
using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application.Models.Projects
{
    /// <summary>
    /// 命令信息
    /// </summary>
    public class CommandInfoModel : DtoBase
    {
        private CommandType _type;
        private int _repeatCount;
        private string _functionName;

        /// <summary>
        /// 类型
        /// </summary>
        public CommandType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public string TypeDescription => Type.Description();

        /// <summary>
        /// 重复次数
        /// </summary>
        public int RepeatCount
        {
            get => _repeatCount;
            set => SetProperty(ref _repeatCount, value);
        }


        /// <summary>
        /// 方法名
        /// </summary>
        public string FunctionName
        {
            get { return _functionName; }
            set { _functionName = value; }
        }

    }
}
