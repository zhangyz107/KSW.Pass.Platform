using KSW.ATE01.Pattern.Application.BLLs.Abstractions.Patterns;
using KSW.ATE01.Pattern.Application.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application.BLLs.Implements.Patterns
{
    internal class DllDataSource : ITestPlanDataSource, IDisposable
    {
        private string _dataSourcePath;

        private string _extension;

        private Dictionary<string, DataTable> _dataTableNameDic = new Dictionary<string, DataTable>();

        public string DataSourcePath => _dataSourcePath;

        public string Extension => _extension;

        public Dictionary<string, DataTable> DataTableNameDic => _dataTableNameDic;

        public DllDataSource(string filePath)
        {
            _dataSourcePath = filePath;
            _extension = Path.GetExtension(filePath);
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
            GetAssemblyResourceFiles(DataSourcePath, ".csv", out var dicResourceNameResourceContent);
            foreach (KeyValuePair<string, string> item in dicResourceNameResourceContent)
            {
                DataTable value = DataTableHelper.FormatAsDataTable(item.Value);
                _dataTableNameDic.Add(Path.GetFileNameWithoutExtension(item.Key), value);
            }
            return true;
        }

        private bool GetAssemblyResourceFiles(string assemblyPath, string resourceNameExtensionFilter, out Dictionary<string, string> dicResourceNameResourceContent)
        {
            dicResourceNameResourceContent = new Dictionary<string, string>();
            Assembly assembly = Assembly.LoadFile(assemblyPath);
            List<string> list = (from x in assembly.GetManifestResourceNames()
                                 where Path.GetExtension(x).Equals(resourceNameExtensionFilter, StringComparison.CurrentCultureIgnoreCase)
                                 select x).ToList();
            if (list.Count == 0)
            {
                throw new FileNotFoundException("Can't find TestPlan resource in CPCode assembly.");
            }
            for (int i = 0; i < list.Count; i++)
            {
                Stream manifestResourceStream = assembly.GetManifestResourceStream(list[i]);
                manifestResourceStream.Position = 0L;
                string resourceFileContent = GetResourceFileContent(manifestResourceStream);
                string key = Path.GetFileNameWithoutExtension(list[i]).Split('.').Last();
                dicResourceNameResourceContent.Add(key, resourceFileContent);
            }
            return true;
        }

        private string GetResourceFileContent(Stream stream)
        {
            List<byte> list = new List<byte>();
            byte[] array = new byte[5242880];
            int num = -1;
            while ((num = stream.Read(array, 0, array.Length)) != 0)
            {
                list.AddRange(array.Take(num));
            }
            return Encoding.UTF8.GetString(list.ToArray());
        }
    }
}
