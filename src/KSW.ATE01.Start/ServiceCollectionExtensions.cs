using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace KSW.ATE01.Start
{
    /// <summary>
    /// 服务集合扩展
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLibrary(this IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));
            return services;
        }
    }
}
