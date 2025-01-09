using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application.Helpers
{
    internal class DataTableHelper
    {
        public static DataTable ToDataTable(DataRow[] rows)
        {
            if (rows != null && rows.Length != 0)
            {
                DataTable dataTable = rows[0].Table.Clone();
                foreach (DataRow row in rows)
                {
                    dataTable.ImportRow(row);
                }
                return dataTable;
            }
            return null;
        }

        public static DataTable RemoveDataRowByIndexList(DataTable sourceDataTable, int[] rowsIndex)
        {
            for (int i = 0; i < rowsIndex.Length; i++)
            {
                sourceDataTable.Rows[rowsIndex[i]].Delete();
            }
            sourceDataTable.AcceptChanges();
            return sourceDataTable;
        }

        public static DataTable ListToDataTable<T>(List<T> entitys)
        {
            if (entitys != null && entitys.Count >= 1)
            {
                Type type = entitys[0].GetType();
                PropertyInfo[] properties = type.GetProperties();
                DataTable dataTable = new DataTable();
                for (int i = 0; i < properties.Length; i++)
                {
                    dataTable.Columns.Add(properties[i].Name);
                }
                {
                    foreach (T entity in entitys)
                    {
                        object obj = entity;
                        if (!(obj.GetType() != type))
                        {
                            object[] array = new object[properties.Length];
                            for (int j = 0; j < properties.Length; j++)
                            {
                                array[j] = properties[j].GetValue(obj, null);
                            }
                            dataTable.Rows.Add(array);
                            continue;
                        }
                        throw new Exception("要转换的集合元素类型不一致");
                    }
                    return dataTable;
                }
            }
            throw new Exception("需转换的集合为空");
        }

        public static T GetParameter<T>(string argParam)
        {
            if (typeof(T).Equals(typeof(int)))
            {
                return (T)(object)Convert.ToInt32(argParam);
            }
            if (typeof(T).Equals(typeof(double)))
            {
                return (T)(object)Convert.ToDouble(argParam);
            }
            if (typeof(T).Equals(typeof(double[])))
            {
                return (T)(object)Convert.ToDouble(argParam);
            }
            if (typeof(T).Equals(typeof(int[])))
            {
                return (T)(object)Convert.ToDouble(argParam);
            }
            if (typeof(T).Equals(typeof(string)))
            {
                return (T)(object)Convert.ToDouble(argParam);
            }
            if (typeof(T).Equals(typeof(string[])))
            {
                return (T)(object)Convert.ToDouble(argParam);
            }
            return (T)(object)null;
        }

        public static DataTable FormatAsDataTable(string strLinesContent)
        {
            return FormatAsDataTable(strLinesContent.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None));
        }

        public static DataTable FormatAsDataTable(IEnumerable<string> listLineContent)
        {
            IEnumerable<string[]> source = listLineContent.Select((string x) => x.Split(','));
            int num = source.Max((string[] x) => x.Length);
            DataTable dataTable = new DataTable();
            for (int i = 0; i < num; i++)
            {
                dataTable.Columns.Add("F" + (i + 1));
            }
            List<DataRow> list = source.Select(delegate (string[] x)
            {
                DataRow dataRow = dataTable.NewRow();
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    dataRow[j] = DBNull.Value;
                }
                for (int k = 0; k < x.Length; k++)
                {
                    if (!string.IsNullOrEmpty(x[k]))
                    {
                        dataRow[k] = x[k];
                    }
                }
                return dataRow;
            }).ToList();
            for (int l = 0; l < list.Count(); l++)
            {
                dataTable.Rows.Add(list[l]);
            }
            return dataTable;
        }
    }
}
