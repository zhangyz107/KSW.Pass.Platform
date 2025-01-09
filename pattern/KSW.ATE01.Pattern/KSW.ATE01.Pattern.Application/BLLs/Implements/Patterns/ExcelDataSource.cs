using KSW.ATE01.Pattern.Application.BLLs.Abstractions.Patterns;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application.BLLs.Implements.Patterns
{
    internal class ExcelDataSource : ITestPlanDataSource, IDisposable
    {
        private string _dataSourcePath;

        private string _extension;

        private Dictionary<string, DataTable> _dataTableNameDic = new Dictionary<string, DataTable>();

        private static OleDbConnection _oleDbConnection;

        public string DataSourcePath => _dataSourcePath;

        public string Extension => _extension;

        public Dictionary<string, DataTable> DataTableNameDic => _dataTableNameDic;

        public ExcelDataSource(string filePath)
        {
            _dataSourcePath = filePath;
            _extension = Path.GetExtension(filePath);
        }

        public bool ParseToDataTable()
        {
            string empty = string.Empty;
            switch (Extension)
            {
                default:
                    empty = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DataSourcePath + ";Extended Properties='Excel 8.0;HDR=no;IMEX=1;'";
                    break;
                case ".xlsx":
                case ".xlsm":
                    empty = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + DataSourcePath + ";Extended Properties='Excel 12.0;HDR=no;IMEX=1;'";
                    break;
                case ".xls":
                    empty = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DataSourcePath + ";Extended Properties='Excel 8.0;HDR=no;IMEX=1;'";
                    break;
            }
            _oleDbConnection = new OleDbConnection(empty);
            _oleDbConnection.Open();
            List<string> dataTableNames = GetDataTableNames(_oleDbConnection);
            Dictionary<string, DataTable> dataTable = GetDataTable(dataTableNames, _oleDbConnection);
            _dataTableNameDic = dataTable;
            return true;
        }

        public List<string> GetDataTableNames()
        {
            return DataTableNameDic.Keys.ToList();
        }

        public void Dispose()
        {
            if (_oleDbConnection != null)
            {
                _oleDbConnection.Dispose();
                _oleDbConnection = null;
            }
        }

        private List<string> GetDataTableNames(OleDbConnection oleDbConnection)
        {
            List<string> list = new List<string>();
            foreach (DataRow row in oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null).Rows)
            {
                string text = row["TABLE_NAME"].ToString();
                if (text.Contains("$") && text.Replace("'", "").EndsWith("$"))
                {
                    text = text.Replace("'", "");
                    text = text.Substring(0, text.Length - 1);
                    list.Add(text);
                }
            }
            return list;
        }

        private Dictionary<string, DataTable> GetDataTable(List<string> listDataTableNames, OleDbConnection oleDbConnection)
        {
            Dictionary<string, DataTable> dictionary = new Dictionary<string, DataTable>();
            for (int i = 0; i < listDataTableNames.Count; i++)
            {
                DataTable dataTable = new DataTable();
                new OleDbDataAdapter(new OleDbCommand("select * from [" + listDataTableNames[i] + "$]", oleDbConnection)).Fill(dataTable);
                dictionary.Add(listDataTableNames[i], dataTable);
            }
            return dictionary;
        }
    }
}
