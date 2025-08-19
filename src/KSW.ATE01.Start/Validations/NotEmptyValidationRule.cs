using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KSW.ATE01.Start.Validations
{
    public class NotEmptyValidationRule : ValidationRule
    {
        protected LanguageManager L = LanguageManager.Instance;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace((value ?? "").ToString())
                ? new ValidationResult(false, L["FieldIsRequired"])
                : ValidationResult.ValidResult;
        }
    }
}
