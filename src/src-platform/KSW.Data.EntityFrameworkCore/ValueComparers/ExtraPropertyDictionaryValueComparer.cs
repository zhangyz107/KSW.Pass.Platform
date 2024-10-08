﻿using KSW.Domain.Extending;

namespace KSW.Data.EntityFrameworkCore.ValueComparers;

/// <summary>
/// 扩展属性值比较器
/// </summary>
public class ExtraPropertyDictionaryValueComparer : ValueComparer<ExtraPropertyDictionary> {
    /// <summary>
    /// 初始化扩展属性值比较器
    /// </summary>
    public ExtraPropertyDictionaryValueComparer()
        : base(
            ( extraProperties1, extraProperties2 ) => GetJson( extraProperties1 ) == GetJson( extraProperties2 ),
            extraProperties => extraProperties.Aggregate( 0, ( key, value ) => HashCode.Combine( key, value.GetHashCode() ) ),
            extraProperties => new ExtraPropertyDictionary( extraProperties ) ) {
    }

    /// <summary>
    /// 获取Json
    /// </summary>
    private static string GetJson( ExtraPropertyDictionary extraProperties ) {
        var options = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
        return Helpers.Json.ToJson( extraProperties, options );
    }
}