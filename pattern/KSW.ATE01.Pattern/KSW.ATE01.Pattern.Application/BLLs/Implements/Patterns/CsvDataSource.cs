using KSW.ATE01.Pattern.Application.BLLs.Abstractions.Patterns;
using KSW.ATE01.Pattern.Application.Helpers;
using System.Data;

namespace KSW.ATE01.Pattern.Application.BLLs.Implements.Patterns
{
    internal class CsvDataSource : ITestPlanDataSource, IDisposable
    {
        private string _dataSourcePath;

        private string _extension;

        private Dictionary<string, DataTable> _dataTableNameDic = new Dictionary<string, DataTable>();

        public string DataSourcePath => _dataSourcePath;

        public string Extension => _extension;

        public Dictionary<string, DataTable> DataTableNameDic => _dataTableNameDic;

        public CsvDataSource(string directoryPath)
        {
            _dataSourcePath = directoryPath;
            _extension = string.Empty;
        }

        public void Dispose()
        {

        }

        public List<string> GetDataTableNames()
        {
            return DataTableNameDic.Keys.ToList();
        }

        public bool ParseToDataTable()
        {
            string[] files = Directory.GetFiles(DataSourcePath, "*.csv", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
            {
                throw new FileNotFoundException("Can't find CSV TestPlan in path " + DataSourcePath + ".");
            }
            for (int i = 0; i < files.Length; i++)
            {
                DataTable value = DataTableHelper.FormatAsDataTable(File.ReadAllText(files[i]));
                _dataTableNameDic.Add(Path.GetFileNameWithoutExtension(files[i]), value);
            }
            return true;
        }
    }
}
