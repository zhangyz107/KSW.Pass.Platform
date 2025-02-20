using System.Text;

namespace KSW.ATE01.Instrument.IO.Models.Instruments
{
    /// <summary>
    /// 发送消息数据模型
    /// </summary>
    public class SendMessageModel
    {
        /// <summary>
        /// Ip地址
        /// </summary>
        public string Address { get; set; }

        public byte[] Message { get; set; }

        /// <summary>
        /// 发送超时,多少ms
        /// </summary>
        public int SendTimeOut { get; set; }

        /// <summary>
        /// 编码格式
        /// </summary>
        public Encoding StringEncoder { get; set; }

        /// <summary>
        /// 是否存在确认值
        /// </summary>
        public bool HasAck { get; set; }
    }
}
