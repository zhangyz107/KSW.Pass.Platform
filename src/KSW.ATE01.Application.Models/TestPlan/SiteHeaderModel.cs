using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Models.TestPlan
{
    /// <summary>
    /// 站点头模型
    /// </summary>
    public class SiteHeaderModel : DtoBase
    {
        private string _siteHeaderName;
        private bool _isSelected;

        /// <summary>
        /// 站点头名称
        /// </summary>
        public string SiteHeaderName
        {
            get => _siteHeaderName;
            set => SetProperty(ref _siteHeaderName, value);
        }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
