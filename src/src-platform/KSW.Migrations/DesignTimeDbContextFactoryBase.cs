using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.IO;

namespace KSW.Migrations
{
    /// <summary>
    /// 设计时数据上下文工厂 - 仅用于自动迁移
    /// </summary>
    public abstract class DesignTimeDbContextFactoryBase<T> : IDesignTimeDbContextFactory<T> where T : DbContext
    {
        /// <summary>
        /// 创建数据上下文
        /// </summary>
        public virtual T CreateDbContext(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder().AddCommandLine(args);
            var configuration = configurationBuilder.Build();

            configuration = configurationBuilder.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), configuration["basePath"]))
                   .AddXmlFile("App.config", optional: false, reloadOnChange: true)
                   .AddCommandLine(args)
                   .Build();

            //Console.WriteLine(this.GetConnectionString(configuration));

            return CreateDbContext(this.GetConnectionString(configuration));
        }


        /// <summary>
        /// 获取连接字符串
        /// </summary>
        public abstract string GetConnectionString(IConfiguration configuration);

        /// <summary>
        /// 创建DbContext
        /// </summary>
        public abstract T CreateDbContext(string connString);
    }
}