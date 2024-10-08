using KSW.Aop;
using KSW.Dependency;

namespace KSW.Application;

/// <summary>
/// 应用服务
/// </summary>
public interface IService : IScopeDependency, IAopProxy
{
}