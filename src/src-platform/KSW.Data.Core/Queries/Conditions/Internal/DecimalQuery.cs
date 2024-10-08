﻿namespace KSW.Data.Queries.Conditions.Internal;

/// <summary>
/// decimal范围查询参数对象 - 使用该对象的目的是构建参数化条件
/// </summary>
internal class DecimalQuery
{
    /// <summary>
    /// 最小值
    /// </summary>
    public decimal? MinValue { get; set; }
    /// <summary>
    /// 最大值
    /// </summary>
    public decimal? MaxValue { get; set; }
}