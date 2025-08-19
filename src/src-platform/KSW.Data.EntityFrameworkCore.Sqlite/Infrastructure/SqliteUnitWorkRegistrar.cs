using AspectCore.DependencyInjection;
using KSW.Helpers;
using KSW.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.Data.EntityFrameworkCore.Infrastructure
{
    public class SqliteUnitWorkRegistrar : IServiceRegistrar
    {
        /// <summary>
        /// 获取服务名
        /// </summary>
        public static string ServiceName => "KSW.Data.EntityFrameworkCore.Sqlite.Infrastructure.SqliteUnitWorkRegistrar";

        /// <summary>
        /// 排序号
        /// </summary>
        public int OrderId => 110;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled => ServiceRegistrarConfig.IsEnabled(ServiceName);

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Action Register(ServiceContext serviceContext)
        {
            // 配置代理生成器
            var containerRegistry = serviceContext.ContainerRegistry;
            var types = serviceContext.TypeFinder.Find<IDatabaseRegistrar>();
            var instances = types.Select(type => Reflection.CreateInstance<IDatabaseRegistrar>(type)).ToList();

            return () =>
            {
                foreach (var instance in instances)
                    instance.RegisterDatabase(containerRegistry);
            };
        }
    }
}
