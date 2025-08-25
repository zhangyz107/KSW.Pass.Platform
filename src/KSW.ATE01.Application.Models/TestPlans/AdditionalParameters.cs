using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Models.TestPlans
{
    /// <summary>
    /// 附加参数
    /// </summary>
    public class AdditionalParameters : BindableBase
    {
        private string _parameter;

        public string Parameter
        {
            get => _parameter;
            set => SetProperty(ref _parameter, value);
        }

    }
}
