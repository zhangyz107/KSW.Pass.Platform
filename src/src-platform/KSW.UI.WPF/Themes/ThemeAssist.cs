using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KSW.UI.WPF.Themes
{
    public static class ThemeAssist
    {
        public static void ChangeTheme(bool isDark)
        {
            var theme = isDark ? "Dark" : "Light";
            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            // 移除旧的主题资源字典 (这里假设通过字典源URI中包含的特定字符串，如 "LightTheme" 或 "DarkTheme" 来识别)
            var oldThemeDict = mergedDictionaries.FirstOrDefault(d => d.Source?.OriginalString?.Contains("LightTheme") == true
                                                                   || d.Source?.OriginalString?.Contains("DarkTheme") == true);
            if (oldThemeDict != null)
            {
                mergedDictionaries.Remove(oldThemeDict);
            }
            // 添加新的主题资源字典
            var newThemeDict = new ResourceDictionary { Source = new Uri($"/KSW.UI.WPF;component/Themes/KSW.PlatformTheme.{theme}Theme.xaml", UriKind.Relative) };
            mergedDictionaries.Add(newThemeDict);
        }
    }
}
