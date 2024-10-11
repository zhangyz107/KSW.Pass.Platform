using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.Localization
{
    public interface ILanguageManager : INotifyPropertyChanged
    {
        /// <summary>
        /// 属性访问器
        /// </summary>
        /// <returns></returns>
        string this[string name] { get; }
        /// <summary>
        /// 切换多语言
        /// </summary>
        void ChangeLanguage(CultureInfo cultureInfo);
    }
}
