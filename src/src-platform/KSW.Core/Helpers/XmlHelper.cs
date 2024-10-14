/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：XmlHelper.cs
// 功能描述：Xml序列化和反序列化帮助类
//
// 作者：zhangyingzhong
// 日期：2024/10/12 16:16
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KSW.Helpers
{
    /// <summary>
    /// Xml序列化和反序列化帮助类
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// 将对象序列化并保存到XML文件
        /// </summary>
        /// <typeparam name="T">需要序列化的对象类型</typeparam>
        /// <param name="obj">需要序列化的对象</param>
        /// <param name="filePath">保存的文件路径</param>
        public static void SerializeToXml<T>(T obj, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            if (serializer != null)
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    serializer.Serialize(fs, obj);
                }
        }

        /// <summary>
        /// 将IList对象序列化并保存到XML文件
        /// </summary>
        /// <typeparam name="T">列表项的类型</typeparam>
        /// <param name="list">需要序列化的列表对象</param>
        /// <param name="filePath">保存的文件路径</param>
        public static void SerializeToXml<T>(IList<T> list, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));

            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                serializer.Serialize(fs, list);
            }
        }

        /// <summary>
        /// 从XML文件反序列化为对象
        /// </summary>
        /// <typeparam name="T">需要反序列化的对象类型</typeparam>
        /// <param name="filePath">XML文件路径</param>
        /// <returns>反序列化后的对象</returns>
        public static T DeserializeFromXml<T>(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                return (T)serializer.Deserialize(fs);
            }
        }

        /// <summary>
        /// 从XML文件反序列化为IList对象
        /// </summary>
        /// <typeparam name="T">列表项的类型</typeparam>
        /// <param name="filePath">XML文件路径</param>
        /// <returns>反序列化后的列表对象</returns>
        public static IList<T> DeserializeListFromXml<T>(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));

            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                return (IList<T>)serializer.Deserialize(fs);
            }
        }
    }
}
