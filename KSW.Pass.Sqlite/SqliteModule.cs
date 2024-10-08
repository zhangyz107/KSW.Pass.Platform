using KSW.Pass.Common.Helper;
using KSW.Pass.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.Pass.Sqlite
{
    public class SqliteModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
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
