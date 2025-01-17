using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application.Models.Instruments
{
    /// <summary>
    /// 发送消息数据模型
    /// </summary>
    public class SendMessageModel : DtoBase
    {
        private string _address;
        private byte[] _message;
        private int _sendTimeOut;
        private Encoding _stringEncoder;

        /// <summary>
        /// Ip地址
        /// </summary>
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }
        public byte[] Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        /// <summary>
        /// 发送超时,多少ms
        /// </summary>
        public int SendTimeOut
        {
            get => _sendTimeOut;
            set => SetProperty(ref _sendTimeOut, value);
        }

        /// <summary>
        /// 编码格式
        /// </summary>
        public Encoding StringEncoder
        {
            get => _stringEncoder;
            set => SetProperty(ref _stringEncoder, value);
        }
    }
}
