using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.Data.EntityFrameworkCore
{
    public interface IDatabaseRegistrar
    {
        /// <summary>
        /// 注册数据库服务
        /// </summary>
        /// <param name="containerRegistry"></param>
        void RegisterDatabase(IContainerRegistry containerRegistry);
    }
}
