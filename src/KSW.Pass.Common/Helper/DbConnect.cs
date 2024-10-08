using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.Pass.Common.Helper
{
    public class DbConnect
    {
        public static string GetDbConnectStatement()
        {
            var connectionName = ConfigurationManager.ConnectionStrings["DatabaseName"].ConnectionString;
            var connectPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KSW_Pass");
            var dir = Path.GetDirectoryName(connectPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return $"Data Source={connectPath}";
        }
    }
}
