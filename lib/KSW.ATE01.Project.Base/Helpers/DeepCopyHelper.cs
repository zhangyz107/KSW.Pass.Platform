/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：DeepCopyHelper.cs
// 功能描述：数据深拷贝帮助类
//
// 作者：zhangyingzhong
// 日期：2025/01/22 10:00
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using System.Reflection;

namespace KSW.ATE01.Project.Base.Helpers
{
    /// <summary>
    /// 数据深拷贝帮助类
    /// </summary>
    public class DeepCopyHelper
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
