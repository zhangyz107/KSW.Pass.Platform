using KSW.Localization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace KSW.ATE01.Start
{
    public class LExtension : LanguageMarkupExtension
    {
        public LExtension(string name) : base(name)
        {

        }

        public override object LanguageSource => LanguageHelper.Manager;
    }
}
