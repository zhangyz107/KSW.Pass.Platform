using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Markup;

namespace KSW.Localization
{
    /// <summary>
    /// 多语言XAML标记扩展
    /// </summary>
    public abstract class LanguageMarkupExtension : MarkupExtension
    {
        private string _name;

        public LanguageMarkupExtension(string name)
        {
            _name = name;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // 创建绑定
            var binding = new Binding
            {
                Path = new PropertyPath($"[{_name}]"),
                Source = LanguageSource
            };

            // 解析当前的提供者
            var targetProperty = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (targetProperty != null)
            {
                // 将绑定设置到目标属性
                return binding.ProvideValue(serviceProvider);
            }
            return null;
        }

        public abstract object LanguageSource { get; }
    }
}
