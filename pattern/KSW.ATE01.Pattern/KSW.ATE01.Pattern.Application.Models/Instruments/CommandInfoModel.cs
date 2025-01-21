using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application.Models.Instruments
{
    /// <summary>
    /// 指令信息模型
    /// </summary>
    public class CommandInfoModel : DtoBase
    {
        private string _commandCode;
        private short _commnadLength;
        private byte[] _commnadContent;

        /// <summary>
        /// 指令Id
        /// </summary>
        public string CommandCode
        {
            get => _commandCode;
            set => SetProperty(ref _commandCode, value);
        }

        /// <summary>
        /// 指令长度
        /// </summary>
        public short CommnadLength => CommandContent.IsEmpty() ? (short)0 : (short)CommandContent.Length;

        /// <summary>
        /// 指令内容
        /// </summary>
        public byte[] CommandContent
        {
            get => _commnadContent;
            set => SetProperty(ref _commnadContent, value);
        }
    }
}
