using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models.Patterns
{
    /// <summary>
    /// 模式向量模型
    /// </summary>
    public struct PatternVectorModel
    {
        /// <summary>
        /// 标签模型
        /// </summary>
        public LabelModel Label;

        /// <summary>
        /// 指令
        /// </summary>
        public CommandModel Command;

        /// <summary>
        /// 设备名
        /// </summary>
        public string InstrumentName;

        /// <summary>
        /// 时钟设置
        /// </summary>
        public string TimingSet;

        /// <summary>
        /// 引脚向量
        /// </summary>
        public List<PinModel> Pins;

        /// <summary>
        /// 评论
        /// </summary>
        public string Comment;


        public PatternVectorModel()
        {
            Pins = new List<PinModel>();
        }
    }
}
