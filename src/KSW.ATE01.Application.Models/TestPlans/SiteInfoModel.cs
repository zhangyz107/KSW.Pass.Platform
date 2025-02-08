using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Models.TestPlans
{
    /// <summary>
    /// 站点头模型
    /// </summary>
    public class SiteInfoModel : DtoBase
    {
        private string _siteName;
        private string _siteValue;
        private bool _isSelected;

        /// <summary>
        /// 站点头名称
        /// </summary>
        public string SiteName
        {
            get => _siteName;
            set => SetProperty(ref _siteName, value);
        }

        /// <summary>
        /// 站点值
        /// </summary>
        public string SiteValue
        {
            get => _siteValue;
            set => SetProperty(ref _siteValue, value);
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
