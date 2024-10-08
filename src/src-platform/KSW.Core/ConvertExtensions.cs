namespace KSW;

/// <summary>
/// 类型转换扩展
/// </summary>
public static class ConvertExtensions
{
    /// <summary>
    /// 安全转换为字符串，去除两端空格，当值为null时返回""
    /// </summary>
    /// <param name="input">输入值</param>
    public static string SafeString(this object input)
    {
        return input?.ToString()?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// 转换为bool
    /// </summary>
    /// <param name="obj">数据</param>
    public static bool ToBool(this string obj)
    {
        return Helpers.Convert.ToBool(obj);
    }

    /// <summary>
    /// 转换为可空bool
    /// </summary>
    /// <param name="obj">数据</param>
    public static bool? ToBoolOrNull(this string obj)
    {
        return Helpers.Convert.ToBoolOrNull(obj);
    }

    /// <summary>
    /// 转换为int
    /// </summary>
    /// <param name="obj">数据</param>
    public static int ToInt(this string obj)
    {
        return Helpers.Convert.ToInt(obj);
    }

    /// <summary>
    /// 转换为可空int
    /// </summary>
    /// <param name="obj">数据</param>
    public static int? ToIntOrNull(this string obj)
    {
        return Helpers.Convert.ToIntOrNull(obj);
    }

    /// <summary>
    /// 转换为long
    /// </summary>
    /// <param name="obj">数据</param>
    public static long ToLong(this string obj)
    {
        return Helpers.Convert.ToLong(obj);
    }

    /// <summary>
    /// 转换为可空long
    /// </summary>
    /// <param name="obj">数据</param>
    public static long? ToLongOrNull(this string obj)
    {
        return Helpers.Convert.ToLongOrNull(obj);
    }

    /// <summary>
    /// 转换为double
    /// </summary>
    /// <param name="obj">数据</param>
    public static double ToDouble(this string obj)
    {
        return Helpers.Convert.ToDouble(obj);
    }

    /// <summary>
    /// 转换为可空double
    /// </summary>
    /// <param name="obj">数据</param>
    public static double? ToDoubleOrNull(this string obj)
    {
        return Helpers.Convert.ToDoubleOrNull(obj);
    }

    /// <summary>
    /// 转换为decimal
    /// </summary>
    /// <param name="obj">数据</param>
    public static decimal ToDecimal(this string obj)
    {
        return Helpers.Convert.ToDecimal(obj);
    }

    /// <summary>
    /// 转换为可空decimal
    /// </summary>
    /// <param name="obj">数据</param>
    public static decimal? ToDecimalOrNull(this string obj)
    {
        return Helpers.Convert.ToDecimalOrNull(obj);
    }

    /// <summary>
    /// 转换为日期
    /// </summary>
    /// <param name="obj">数据</param>
    public static DateTime ToDateTime(this string obj)
    {
        return Helpers.Convert.ToDateTime(obj);
    }

    /// <summary>
    /// 转换为可空日期
    /// </summary>
    /// <param name="obj">数据</param>
    public static DateTime? ToDateTimeOrNull(this string obj)
    {
        return Helpers.Convert.ToDateTimeOrNull(obj);
    }

    /// <summary>
    /// 转换为Guid
    /// </summary>
    /// <param name="obj">数据</param>
    public static Guid ToGuid(this string obj)
    {
        return Helpers.Convert.ToGuid(obj);
    }

    /// <summary>
    /// 转换为可空Guid
    /// </summary>
    /// <param name="obj">数据</param>
    public static Guid? ToGuidOrNull(this string obj)
    {
        return Helpers.Convert.ToGuidOrNull(obj);
    }

    /// <summary>
    /// 转换为Guid集合
    /// </summary>
    /// <param name="obj">数据,范例: "83B0233C-A24F-49FD-8083-1337209EBC9A,EAB523C6-2FE7-47BE-89D5-C6D440C3033A"</param>
    public static List<Guid> ToGuidList(this string obj)
    {
        return Helpers.Convert.ToGuidList(obj);
    }

    /// <summary>
    /// 转换为Guid集合
    /// </summary>
    /// <param name="obj">字符串集合</param>
    public static List<Guid> ToGuidList(this IList<string> obj)
    {
        if (obj == null)
            return new List<Guid>();
        return obj.Select(t => t.ToGuid()).ToList();
    }
}