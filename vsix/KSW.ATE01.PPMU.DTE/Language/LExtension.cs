using KSW.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.PPMU.Start.Language
{
    public class LExtension : LanguageMarkupExtension
    {
        public LExtension(string name) : base(name)
        {

        }

        public override object LanguageSource => LanguageManager.Instance;
    }
}
