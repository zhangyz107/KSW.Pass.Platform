using KSW.ATE01.Project.Base.Enums.Patterns;
using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Models.Patterns
{
    /// <summary>
    /// 引脚信息模型
    /// </summary>
    public class PinModel : DtoBase
    {
        private string _pinNmae;
        private VectorValueType _vectorValue;

        /// <summary>
        /// 引脚名
        /// </summary>
        public string PinName
        {
            get => _pinNmae;
            set => SetProperty(ref _pinNmae, value);
        }

        /// <summary>
        /// 向量值
        /// </summary>
        [Required(ErrorMessage = "TheFieldRequired")]
        public VectorValueType VectorValue
        {
            get => _vectorValue;
            set => SetProperty(ref _vectorValue, value);
        }

        public string VectorValueDescription => VectorValue.Description();
    }
}
