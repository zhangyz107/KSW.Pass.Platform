using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Helpers
{
    public static class EnumHelper
    {
        public static Dictionary<string, T> GetEnumDescriptionDic<T>() where T : Enum
        {
            var result = new Dictionary<string, T>();
            var values = Enum.GetValues(typeof(T));
            foreach (var value in values)
            {
                if (value is T enumValue)
                    result.Add(enumValue.GetDescription(), enumValue);
            }
            return result;
        }

        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            if (field != null)
            {
                var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
                if (attribute != null)
                {
                    return attribute.Description;
                }
            }
            return value.ToString(); // 如果没有描述，返回枚举名
        }

        public static TEnum GetEnumValueFromDescription<TEnum>(string description) where TEnum : Enum
        {
            // 获取枚举类型
            Type type = typeof(TEnum);

            // 遍历所有枚举值
            foreach (var field in type.GetFields())
            {
                // 查找DescriptionAttribute特性
                var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
                if (attribute != null && attribute.Description == description)
                {
                    return (TEnum)field.GetValue(null);
                }
            }

            // 如果未找到匹配的描述，抛出异常
            throw new ArgumentException($"No enum value found for description '{description}'", nameof(description));
        }
    }
}
