using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace KSW.ATE01.Start
{
    public class LExtension : MarkupExtension
    {
        private string _name;

        public LExtension(string name)
        {
            _name = name;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // 创建绑定
            var binding = new Binding
            {
                Path = new PropertyPath($"[{_name}]"),
                Source = LanguageManager.Instance
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
    }
}
