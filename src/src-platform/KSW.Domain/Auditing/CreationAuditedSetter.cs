using KSW.Helpers;

namespace KSW.Domain.Auditing;

/// <summary>
/// 创建操作审计设置器
/// </summary>
public class CreationAuditedSetter
{
    /// <summary>
    /// 实体
    /// </summary>
    private readonly object _entity;

    /// <summary>
    /// 初始化创建操作审计设置器
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="userId">用户标识</param>
    private CreationAuditedSetter(object entity)
    {
        _entity = entity;
    }

    /// <summary>
    /// 设置创建审计属性
    /// </summary>
    /// <param name="entity">实体</param>
    public static void Set(object entity)
    {
        new CreationAuditedSetter(entity).Init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        if (_entity == null)
            return;
        InitCreationTime();
    }

    /// <summary>
    /// 初始化创建时间
    /// </summary>
    private void InitCreationTime()
    {
        if (_entity is ICreationTime entity)
            entity.CreationTime ??= Time.Now;
    }

    /// <summary>
    /// 创建时间是否为空
    /// </summary>
    private bool IsEmpty<T>(T creatorId)
    {
        if (creatorId == null)
            return true;
        if (creatorId.Equals(default(T)))
            return true;
        return false;
    }
}