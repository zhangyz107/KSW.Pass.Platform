using KSW.Pass.Data;
using Microsoft.EntityFrameworkCore;
using Util.Data.EntityFrameworkCore;

namespace KSW.Pass.Sqlite
{
    public class SystemUnitOfWork : SqliteUnitOfWorkBase, ISystemUnitOfWork
    {
        public SystemUnitOfWork(DbContextOptions options) : base(options)
        {
        }
    }
}
