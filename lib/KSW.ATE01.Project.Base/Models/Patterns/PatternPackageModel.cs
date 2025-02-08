using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models.Patterns
{
    /// <summary>
    /// Pattern包模型
    /// </summary>
    public class PatternPackageModel
    {
        /// <summary>
        /// 通道号
        /// </summary>
        public int ChannelNum { get; set; }

        /// <summary>
        /// 起始地址
        /// </summary>
        public byte[] Address { get; set; }

        /// <summary>
        /// 数据长度
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// 数据长度（字节表示）
        /// </summary>
        public byte[] LengthBytes { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public List<PatternGroupModel> PatternGroups { get; set; }
    }
}
