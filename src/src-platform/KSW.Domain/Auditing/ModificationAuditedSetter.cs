using KSW.Helpers;

namespace KSW.Domain.Auditing;

/// <summary>
/// 修改操作审计设置器
/// </summary>
public class ModificationAuditedSetter
{
    /// <summary>
    /// 实体
    /// </summary>
    private readonly object _entity;

    /// <summary>
    /// 初始化修改操作审计设置器
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="userId">用户标识</param>
    private ModificationAuditedSetter(object entity)
    {
        _entity = entity;
    }

    /// <summary>
    /// 设置修改审计属性
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="userId">用户标识</param>
    public static void Set(object entity)
    {
        new ModificationAuditedSetter(entity).Init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        if (_entity == null)
            return;
        InitLastModificationTime();
    }

    /// <summary>
    /// 初始化最后修改时间
    /// </summary>
    private void InitLastModificationTime()
    {
        if (_entity is ILastModificationTime entity)
            entity.LastModificationTime = Time.Now;
    }
}