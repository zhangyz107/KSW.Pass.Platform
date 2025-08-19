using KSW.ATE01.Common.Helper;
using KSW.ATE01.Data;
using KSW.Data.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Sqlite
{
    /// <summary>
    /// Sqlite数据库注册
    /// </summary>
    public class SqliteRegistrar : IDatabaseRegistrar
    {
        public void RegisterDatabase(IContainerRegistry containerRegistry)
        {
            var connectStatement = DbConnect.GetDbConnectStatement();

            // 使用自定义选项创建 DbContextOptions
            var options = new DbContextOptionsBuilder()
                              .UseSqlite(connectStatement)
                              .Options;

            containerRegistry.RegisterInstance(options);
            containerRegistry.RegisterScoped<ISystemUnitOfWork, SystemUnitOfWork>();
        }
    }
}
