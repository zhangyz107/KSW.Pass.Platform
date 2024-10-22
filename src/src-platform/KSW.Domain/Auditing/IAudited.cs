namespace KSW.Domain.Auditing;

/// <summary>
/// 操作审计
/// </summary>
public interface IAudited : ICreationTime, ILastModificationTime
{

}