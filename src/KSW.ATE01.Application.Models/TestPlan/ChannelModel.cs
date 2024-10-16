using KSW.ATE01.Domain.TestPlan.Core.Enums;
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
    /// 通道模型
    /// </summary>
    public class ChannelModel : DtoBase
    {
        private Guid _groupId;
        private string _groupName;
        private Guid _pinId;
        private string _pinName;
        private ChannelType _type;
        private List<SiteModel> _sites = new List<SiteModel>();

        /// <summary>
        /// 组Id
        /// </summary>
        public Guid GroupId
        {
            get => _groupId;
            set => SetProperty(ref _groupId, value);
        }

        /// <summary>
        /// 组名
        /// </summary>
        public string GroupName
        {
            get => _groupName;
            set => SetProperty(ref _groupName, value);
        }

        /// <summary>
        /// 引脚Id
        /// </summary>
        public Guid PinId
        {
            get => _pinId;
            set => SetProperty(ref _pinId, value);
        }

        /// <summary>
        /// 引脚名称
        /// </summary>
        public string PinName
        {
            get => _pinName;
            set => SetProperty(ref _pinName, value);
        }

        /// <summary>
        /// 通道类型
        /// </summary>
        public ChannelType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        /// <summary>
        /// Sites信息
        /// </summary>
        public List<SiteModel> Sites
        {
            get => _sites;
            set => SetProperty(ref _sites, value);
        }
    }
}
