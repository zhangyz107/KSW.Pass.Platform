using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Models.TestPlan
{
    /// <summary>
    /// 流程模型
    /// </summary>
    public class FlowModel : DtoBase
    {
        private Guid _testItemId;
        private int _sortId;
        private bool _enable;

        /// <summary>
        /// 测试项Id
        /// </summary>
        public Guid TestItemId 
        { 
            get => _testItemId; 
            set => SetProperty(ref _testItemId, value);
        }

        /// <summary>
        /// 序号
        /// </summary>
        public int SortId 
        {
            get => _sortId; 
            set => SetProperty(ref _sortId, value); 
        }

        /// <summary>
        /// 使能
        /// </summary>
        public bool Enable 
        { 
            get => _enable; 
            set => SetProperty(ref _enable, value);
        }
    }
}
