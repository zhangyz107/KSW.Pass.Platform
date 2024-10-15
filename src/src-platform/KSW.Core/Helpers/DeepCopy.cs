using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.Helpers
{
    /// <summary>
    /// 数据深拷贝帮助类
    /// </summary>
    public static class DeepCopy
    {
        public static T Copy<T>(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            // 创建对象的副本
            return (T)DeepCopyObject(obj);
        }

        private static object DeepCopyObject(object obj)
        {
            if (obj == null) return null;

            Type type = obj.GetType();

            // 如果是值类型或字符串，直接返回副本
            if (type.IsValueType || obj is string)
            {
                return obj;
            }

            // 如果是集合，处理集合类型
            if (obj is IEnumerable<object> collection)
            {
                var newList = Activator.CreateInstance(type) as IList<object>;
                foreach (var item in collection)
                {
                    newList.Add(DeepCopyObject(item));
                }
                return newList;
            }

            // 创建该对象的副本
            var newObject = Activator.CreateInstance(type);

            // 复制字段和属性
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                field.SetValue(newObject, DeepCopyObject(field.GetValue(obj)));
            }

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                          .Where(p => p.CanWrite))
            {
                property.SetValue(newObject, DeepCopyObject(property.GetValue(obj)));
            }

            return newObject;
        }
    }
}
