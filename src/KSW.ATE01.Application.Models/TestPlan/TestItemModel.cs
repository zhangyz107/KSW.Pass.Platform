using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Models.TestPlan
{
    /// <summary>
    /// 测试项模型
    /// </summary>
    public class TestItemModel : DtoBase
    {
        private Guid _id;
        private string _testItemName;
        private string _functionName;
        private decimal _force;
        private string _pins;
        private List<LevelModel> _levels = new List<LevelModel>();
        private List<TimingModel> _timings = new List<TimingModel>();
        private List<string> _args = new List<string>();

        /// <summary>
        /// 测试项Id
        /// </summary>
        public Guid Id 
        { 
            get => _id;
            set => SetProperty(ref _id, value);
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
        /// 方法名
        /// </summary>
        public string FunctionName 
        { 
            get => _functionName; 
            set => SetProperty(ref _functionName, value); 
        }

        /// <summary>
        /// 强制值
        /// </summary>
        public decimal Force
        {
            get => _force;
            set => SetProperty(ref _force, value);
        }

        /// <summary>
        /// 待测引脚
        /// </summary>
        public string Pins
        { 
            get => _pins; 
            set => SetProperty(ref _pins, value); 
        }

        /// <summary>
        /// 电压参数集合
        /// </summary>
        public List<LevelModel> Levels 
        { 
            get => _levels; 
            set => SetProperty(ref _levels, value); 
        }

        /// <summary>
        /// 时钟参数集合
        /// </summary>
        public List<TimingModel> Timings 
        {
            get => _timings;
            set => SetProperty(ref _timings, value); 
        }

        /// <summary>
        /// 参数
        /// </summary>
        public List<string> Args
        { 
            get => _args; 
            set => SetProperty(ref _args, value); 
        }
    }
}
