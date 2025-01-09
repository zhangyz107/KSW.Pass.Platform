using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application.BLLs.Abstractions.Patterns
{
    internal interface ITestPlanDataSource : IDisposable
    {
        string DataSourcePath { get; }

        string Extension { get; }

        Dictionary<string, DataTable> DataTableNameDic { get; }

        bool ParseToDataTable();

        List<string> GetDataTableNames();
    }
}
