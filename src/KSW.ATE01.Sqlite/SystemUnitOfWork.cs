using KSW.ATE01.Data;
using Microsoft.EntityFrameworkCore;
using Util.Data.EntityFrameworkCore;

namespace KSW.ATE01.Sqlite
{
    public class SystemUnitOfWork : SqliteUnitOfWorkBase, ISystemUnitOfWork
    {
        public SystemUnitOfWork(DbContextOptions options) : base(options)
        {
        }
    }
}
