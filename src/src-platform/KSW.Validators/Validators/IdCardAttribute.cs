﻿using KSW.Properties;

namespace KSW.Validators.Validators;

/// <summary>
/// 身份证验证
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class IdCardAttribute : ValidationAttribute
{
    /// <summary>
    /// 格式化错误消息
    /// </summary>
    public override string FormatErrorMessage(string name)
    {
        if (ErrorMessage == null && ErrorMessageResourceName == null)
            ErrorMessage = R.InvalidIdCard;
        return string.Format(CultureInfo.CurrentCulture, ErrorMessageString);
    }

    /// <summary>
    /// 是否验证通过
    /// </summary>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value.SafeString().IsEmpty())
            return ValidationResult.Success;
        if (Helpers.Regex.IsMatch(value.SafeString(), ValidatePattern.IdCardPattern))
            return ValidationResult.Success;
        return new ValidationResult(FormatErrorMessage(string.Empty));
    }
}