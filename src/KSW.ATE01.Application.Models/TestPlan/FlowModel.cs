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
        private string _testItemName;
        private string _sheetName;
        private int _sortId;
        private string _enable;
        private bool _isSelected;

        /// <summary>
        /// 测试项Id
        /// </summary>
        public Guid TestItemId
        {
            get => _testItemId;
            set => SetProperty(ref _testItemId, value);
        }

        /// <summary>
        /// 测试项名称
        /// </summary>
        public string TestItemName
        {
            get => _testItemName;
            set => SetProperty(ref _testItemName, value);
        }

        /// <summary>
        /// Sheet名称
        /// </summary>
        public string SheetName
        {
            get => _sheetName;
            set => SetProperty(ref _sheetName, value);
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
        public string Enable
        {
            get => _enable;
            set => SetProperty(ref _enable, value);
        }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

    }
}
