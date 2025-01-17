using KSW.ATE01.Pattern.Domain.Projects.Core.Enums;
using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application.Models.Projects
{
    public class LabelModel : DtoBase
    {
        private long _indexInVectors;
        private LabelCommandType _labelType;
        private string _labelFullContent;
        private object _labelParamter;

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

        /// <summary>
        /// 标签参数
        /// </summary>
        public object LabelParamter
        {
            get => _labelParamter;
            set => SetProperty(ref _labelParamter, value);
        }

    }
}
