using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Helpers
{
    /// <summary>
    /// 类型转换
    /// </summary>
    public static class Convert
    {
        /// <summary>
        /// 获取类型
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        public static Type GetType<T>()
        {
            return GetType(typeof(T));
        }

        /// <summary>
        /// 获取类型
        /// </summary>
        /// <param name="type">类型</param>
        public static Type GetType(Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        #region ToGuid(转换为Guid)

        /// <summary>
        /// 转换为Guid
        /// </summary>
        /// <param name="obj">数据</param>
        public static Guid ToGuid(this string obj)
        {
            if (obj == null)
                return Guid.Empty;
            var inputStr = obj?.Trim() ?? string.Empty;
            if (Guid.TryParse(inputStr, out Guid result))
                return result;
            return Guid.Empty;
        }
        #endregion
    }
}
