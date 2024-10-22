using KSW.ATE01.Common.Helper;
using KSW.Helpers;
using KSW.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace KSW.ATE01.Sqlite
{
    public class SqliteDesignTimeDbContextFactory : DesignTimeDbContextFactoryBase<SystemUnitOfWork>
    {
        public override SystemUnitOfWork CreateDbContext(string connString)
        {
            var connectPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KSW_ATE01", connString);
            var dir = Path.GetDirectoryName(connectPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var connectStatement = $"Data Source={connectPath}";

            // 使用自定义选项创建 DbContextOptions
            var options = new DbContextOptionsBuilder()
                              .UseSqlite(connectStatement)
                              .Options;

            var containerProvider = Ioc.GetContainerProvider();
            return new SystemUnitOfWork(containerProvider, options);
        }

        public override string GetConnectionString(IConfiguration configuration)
        {
            return configuration["connectionStrings:add:DatabaseName:connectionString"];
        }
    }
}
