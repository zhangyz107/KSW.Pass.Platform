using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application.Models.Projects
{
    /// <summary>
    /// 模式向量模型
    /// </summary>
    public class PatternVectorModel : DtoBase
    {
        /// <summary>
        /// 标签模型
        /// </summary>
        public LabelModel Label { get; set; }

        /// <summary>
        /// 指令
        /// </summary>
        public CommandModel Command { get; set; }

        /// <summary>
        /// 设备名
        /// </summary>
        public string InstrumentName { get; set; }

        /// <summary>
        /// 时钟设置
        /// </summary>
        public string TimingSet { get; set; }

        /// <summary>
        /// 引脚向量
        /// </summary>
        public List<PinModel> Pins { get; set; } = new List<PinModel>();

        /// <summary>
        /// 评论
        /// </summary>
        public string Comment { get; set; }
    }
}
