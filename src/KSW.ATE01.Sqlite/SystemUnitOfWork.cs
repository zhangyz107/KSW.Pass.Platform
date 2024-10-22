using KSW.ATE01.Data;
using Microsoft.EntityFrameworkCore;
using Util.Data.EntityFrameworkCore;

namespace KSW.ATE01.Sqlite
{
    public class SystemUnitOfWork : SqliteUnitOfWorkBase, ISystemUnitOfWork
    {
        public SystemUnitOfWork(IContainerProvider containerProvider, DbContextOptions options) : base(containerProvider, options)
        {
        }
    }
}
