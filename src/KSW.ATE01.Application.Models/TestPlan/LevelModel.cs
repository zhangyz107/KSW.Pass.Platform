using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Models.TestPlan
{
    /// <summary>
    /// 电压模型
    /// </summary>
    public class LevelModel : DtoBase
    {
        private Guid _channelGroupId;
        private decimal _vil;
        private decimal _vih;
        private decimal _vol;
        private decimal _voh;
        private decimal _iol;
        private decimal _ioh;
        private decimal _vt;
        private decimal _vcl;
        private decimal _vch;

        /// <summary>
        /// 组Id
        /// </summary>
        public Guid ChannelGroupId 
        {
            get => _channelGroupId; 
            set => SetProperty(ref _channelGroupId, value);
        }

        /// <summary>
        /// 输入低电压
        /// </summary>
        public decimal Vil 
        { 
            get => _vil; 
            set => SetProperty(ref  _vil, value); 
        }

        /// <summary>
        /// 输入高电压
        /// </summary>
        public decimal Vih 
        {
            get => _vih; 
            set => SetProperty(ref _vih, value);
        }

        /// <summary>
        /// 输出低电压
        /// </summary>
        public decimal Vol
        { 
            get => _vol;
            set => SetProperty(ref _vol, value); 
        }

        /// <summary>
        /// 输出高电压
        /// </summary>
        public decimal Voh 
        { 
            get => _voh; 
            set => SetProperty(ref _voh, value); 
        }

        /// <summary>
        /// 输出低电流
        /// </summary>
        public decimal Iol 
        { 
            get => _iol;
            set => SetProperty(ref _iol, value);
        }

        /// <summary>
        /// 输出高电流
        /// </summary>
        public decimal Ioh 
        { 
            get => _ioh;
            set => SetProperty(ref _ioh, value);
        }

        /// <summary>
        /// 电压基准
        /// </summary>
        public decimal Vt 
        {
            get => _vt;
            set => SetProperty(ref _vt, value); 
        }

        /// <summary>
        /// 钳位低电压
        /// </summary>
        public decimal Vcl 
        { 
            get => _vcl; 
            set => SetProperty(ref _vcl, value);
        }

        /// <summary>
        /// 钳位高电压
        /// </summary>
        public decimal Vch 
        {
            get => _vch; 
            set => SetProperty(ref _vch, value); 
        }
    }
}
