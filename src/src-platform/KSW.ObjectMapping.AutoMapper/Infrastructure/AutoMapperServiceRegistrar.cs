﻿using KSW.Helpers;
using KSW.Infrastructure;

namespace KSW.ObjectMapping.AutoMapper.Infrastructure;

/// <summary>
/// AutoMapper服务注册器
/// </summary>
public class AutoMapperServiceRegistrar : IServiceRegistrar
{
    /// <summary>
    /// 获取服务名
    /// </summary>
    public static string ServiceName => "KSW.ObjectMapping.Infrastructure.AutoMapperServiceRegistrar";

    /// <summary>
    /// 排序号
    /// </summary>
    public int OrderId => 300;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled => ServiceRegistrarConfig.IsEnabled(ServiceName);

    /// <summary>
    /// 注册服务
    /// </summary>
    /// <param name="serviceContext">服务上下文</param>
    public Action Register(ServiceContext serviceContext)
    {
        var types = serviceContext.TypeFinder.Find<IAutoMapperConfig>();
        var instances = types.Select(type => Reflection.CreateInstance<IAutoMapperConfig>(type)).ToList();
        var expression = new MapperConfigurationExpression();
        instances.ForEach(t => t.Config(expression));
        var mapper = new ObjectMapper(expression);
        ObjectMapperExtensions.SetMapper(mapper);
        serviceContext.ContainerRegistry.TryRegisterInstance<IObjectMapper>(mapper);
        return null;
    }
}