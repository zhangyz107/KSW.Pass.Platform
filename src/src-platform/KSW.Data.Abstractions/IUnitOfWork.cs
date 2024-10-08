namespace KSW.Data.Abstractions;

/// <summary>
/// 工作单元
/// </summary>
[Aop.Ignore]
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// 提交,返回影响的行数
    /// </summary>
    Task<int> CommitAsync();
}