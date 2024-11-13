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
    /// 引脚信息模型
    /// </summary>
    public class PinInfoModel : DtoBase
    {
        private string _pinName;
        private VectorValueType _vectorValue;

        /// <summary>
        /// 引脚名
        /// </summary>
        public string PinName
        {
            get => _pinName;
            set => SetProperty(ref _pinName, value);
        }

        /// <summary>
        /// 向量值
        /// </summary>
        public VectorValueType VectorValue
        {
            get => _vectorValue;
            set => SetProperty(ref _vectorValue, value);
        }

        public string VectorValueDescription => VectorValue.Description();
    }
}
