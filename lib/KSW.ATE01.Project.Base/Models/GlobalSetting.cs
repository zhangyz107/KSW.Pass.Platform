using KSW.ATE01.Project.Base.Models.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models
{
    public class GlobalSetting : MarshalByRefObject
    {
        private static object _lock = new object();
        private static GlobalSetting _instance;

        private GlobalSetting()
        {
            
        }

        public static GlobalSetting Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new GlobalSetting();
                    }
                    return _instance;
                }
            }
        }


        /// <summary>
        /// 项目信息
        /// </summary>
        public ProjectInfo ProjectInfo { get; set; }

        /// <summary>
        /// 开始测试时间
        /// </summary>
        public DateTime StartTestTime { get; set; }


    }
}
