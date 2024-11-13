using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application.Models.Projects
{
    public class PatternInfoModel : DtoBase
    {
        private int _vector;
        private string _label;
        private CommandInfoModel _command;
        private string _instrument;
        private string _timingName;
        private List<PinInfoModel> _pinInfos;
        private string _comment;

        /// <summary>
        /// 向量
        /// </summary>
        public int Vector
        {
            get => _vector;
            set => SetProperty(ref _vector, value);
        }

        /// <summary>
        /// 标签
        /// </summary>
        public string Label
        {
            get => _label;
            set => SetProperty(ref _label, value);
        }

        /// <summary>
        /// 命令信息
        /// </summary>
        public CommandInfoModel Command
        {
            get => _command;
            set => SetProperty(ref _command, value);
        }

        /// <summary>
        /// 设备
        /// </summary>
        public string Instrument
        {
            get => _instrument;
            set => SetProperty(ref _instrument, value);
        }

        /// <summary>
        /// Timing名
        /// </summary>
        public string TimingName
        {
            get => _timingName;
            set => SetProperty(ref _timingName, value);
        }

        /// <summary>
        /// 引脚向量信息
        /// </summary>
        public List<PinInfoModel> PinInfos
        {
            get => _pinInfos;
            set => SetProperty(ref _pinInfos, value);
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Comment
        {
            get => _comment;
            set => SetProperty(ref _comment, value);
        }

    }
}
