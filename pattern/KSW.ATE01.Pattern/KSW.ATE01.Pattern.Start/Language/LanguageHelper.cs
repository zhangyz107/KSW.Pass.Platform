using KSW.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Start.Language
{
    public class LanguageHelper
    {
        /// <summary>
        /// 多语言资源命名空间
        /// </summary>
        public string ResourceName
        {
            get => "KSW.ATE01.Pattern.Start.Properties.Resources";
        }
        private readonly ILanguageManager _manager;

        private static readonly Lazy<LanguageHelper> _lazy = new Lazy<LanguageHelper>(() => new LanguageHelper());

        public static ILanguageManager Manager { get { return _lazy?.Value?._manager; } }

        public LanguageHelper()
        {
            _manager = LanguageManagerFactory.CreateManager(ResourceName, GetType().Assembly);
        }
    }
}
