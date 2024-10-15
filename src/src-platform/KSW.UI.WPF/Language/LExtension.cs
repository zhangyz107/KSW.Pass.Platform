using KSW.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace KSW.UI.WPF.Language
{
    public class LExtension : LanguageMarkupExtension
    {
        public LExtension(string name) : base(name)
        {

        }

        public override object LanguageSource => LanguageManager.Instance;
    }
}
