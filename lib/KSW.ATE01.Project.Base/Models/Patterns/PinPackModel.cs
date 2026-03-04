using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models.Patterns
{
    [Serializable]
    public class PinPackModel
    {
        /// <summary>
        /// 名称长度
        /// </summary>
        public int NameLength { get; set; }

        /// <summary>
        /// 引脚名称
        /// </summary>
        public string PinName { get; set; }

        /// <summary>
        /// 时间设置长度
        /// </summary>
        public int TimingSetLength { get; set; }

        /// <summary>
        /// 时间设置
        /// </summary>
        public string TimingSet { get; set; }

        /// <summary>
        /// 数据长度
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// 向量数据
        /// </summary>
        public List<PatternVectorGroupModel> Data { get; set; } = new List<PatternVectorGroupModel>();
    }
}
