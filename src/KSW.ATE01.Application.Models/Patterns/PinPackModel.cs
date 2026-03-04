using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Models.Patterns
{
    /// <summary>
    /// 引脚包模型
    /// </summary>
    [Serializable]
    public class PinPackModel : DtoBase
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
        /// 数据长度
        /// </summary>
        public long Length { get; set; }
        /// <summary>
        /// 向量数据
        /// </summary>
        public List<PatternVectorGroupModel> Data { get; set; } = new List<PatternVectorGroupModel>();
    }
}
