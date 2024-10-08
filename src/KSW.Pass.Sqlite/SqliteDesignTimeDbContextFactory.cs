using KSW.Pass.Common.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace KSW.Pass.Sqlite
{
    public class SqliteDesignTimeDbContextFactory : IDesignTimeDbContextFactory<SystemUnitOfWork>
    {
        public SystemUnitOfWork CreateDbContext(string[] args)
        {
            var connectStatement = DbConnect.GetDbConnectStatement();

            // 使用自定义选项创建 DbContextOptions
            var options = new DbContextOptionsBuilder()
                              .UseSqlite(connectStatement)
                              .Options;
            return new SystemUnitOfWork(options);
        }
    }
}
