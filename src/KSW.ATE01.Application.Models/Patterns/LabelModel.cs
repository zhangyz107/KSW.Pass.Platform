using KSW.ATE01.Project.Base.Enums.Patterns;
using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Models.Patterns
{
    /// <summary>
    /// 标签模型
    /// </summary>
    public class LabelModel : DtoBase
    {
        private string _labelName;
        private long _indexInVectors;
        private LabelCommandType _labelType;
        private string _labelFullContent;

        /// <summary>
        /// 标签名称
        /// </summary>
        public string LabelName
        {
            get => _labelName;
            set => SetProperty(ref _labelName, value);
        }

        /// <summary>
        /// 向量序号
        /// </summary>
        public long IndexInVectors
        {
            get => _indexInVectors;
            set => SetProperty(ref _indexInVectors, value);
        }

        /// <summary>
        /// 标签类型
        /// </summary>
        public LabelCommandType LabelType
        {
            get => _labelType;
            set => SetProperty(ref _labelType, value);
        }

        /// <summary>
        /// 标签全内容
        /// </summary>
        public string LabelFullContent
        {
            get => _labelFullContent;
            set => SetProperty(ref _labelFullContent, value);
        }
    }
}
