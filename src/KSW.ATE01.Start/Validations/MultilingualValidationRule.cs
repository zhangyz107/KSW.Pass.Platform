using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KSW.ATE01.Start.Validations
{
    public class MultilingualValidationRule : ValidationRule
    {
        protected LanguageManager L = LanguageManager.Instance;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var stringValue = value as string;
            return string.IsNullOrWhiteSpace((stringValue ?? ""))
                ? ValidationResult.ValidResult
                : new ValidationResult(ValidationResult.ValidResult.IsValid, L[stringValue]);
        }
    }
}
