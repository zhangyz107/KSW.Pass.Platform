namespace KSW.ATE01.Instrument.IO.Models.Instruments
{
    /// <summary>
    /// 记录消息信息
    /// </summary>
    public class RecordMessageModel
    {
        /// <summary>
        /// 记录时间
        /// </summary>
        public DateTime RecordTime { get; set; }

        /// <summary>
        /// 记录内容
        /// </summary>
        public string RecordMessage { get; set; }
    }
}
