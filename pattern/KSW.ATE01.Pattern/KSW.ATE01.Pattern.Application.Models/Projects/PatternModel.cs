using KSW.ATE01.Pattern.Domain.Projects.Core.Enums;
using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application.Models.Projects
{
    /// <summary>
    /// 向量信息模型
    /// </summary>
    public class PatternModel : DtoBase
    {
        /// <summary>
        /// 时钟设置
        /// </summary>
        public List<string> TimingSets { get; set; } = new List<string>();

        /// <summary>
        /// 设备名
        /// </summary>
        public string InstrumentName { get; set; }

        /// <summary>
        /// 设备信息
        /// </summary>
        public InstrumentModel InstrumentInfo { get; set; }

        /// <summary>
        /// 模块类型
        /// </summary>
        public ModuleType ModuleType { get; set; }

        /// <summary>
        /// 模式向量集合
        /// </summary>
        public List<PatternVectorModel> PatternVectors { get; set; } = new List<PatternVectorModel>();
    }
}
