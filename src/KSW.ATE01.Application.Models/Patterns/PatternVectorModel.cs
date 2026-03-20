using KSW.Dtos;
using KSW.Language;
using KSW.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Models.Patterns
{
    /// <summary>
    /// 模式向量模型
    /// </summary>
    public class PatternVectorModel : DtoBase
    {
        private string _timingSet;

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
        [Required(ErrorMessage = "TheFieldRequired")]
        public string TimingSet
        {
            get => _timingSet;
            set => SetProperty(ref _timingSet, value);
        }

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
