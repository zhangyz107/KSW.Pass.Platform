using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Models.TestPlans
{
    /// <summary>
    /// 测试项水平模型
    /// </summary>
    public class LevelModel : DtoBase
    {
        private int _sortId;
        private Guid? _levelGroupId;
        private Guid? _groupOrPinId;
        private decimal? _vil;
        private decimal? _vih;
        private decimal? _vol;
        private decimal? _voh;
        private decimal? _iol;
        private decimal? _ioh;
        private decimal? _vt;
        private decimal? _vcl;
        private decimal? _vch;
        private decimal? _ps;
        private decimal? _i;
        private int? _tdelay;
        private int? _sequence;
        private string _comment;
        private DateTime? _creationTime;
        private DateTime? _lastModificationTime;

        /// <summary>
        /// 排序Id
        /// </summary>
        public int SortId
        {
            get => _sortId;
            set=> SetProperty(ref _sortId, value);
        }

        /// <summary>
        /// 测试项电平组Id
        /// </summary>
        public Guid? LevelGroupId
        {
            get => _levelGroupId;
            set
            {
                if (value != null)
                    SetProperty(ref _levelGroupId, value);
            }
        }

        /// <summary>
        /// 测试计划电平组名称
        /// </summary>
        public string LevelGroupName { get; set; }

        /// <summary>
        /// 组或引脚Id
        /// </summary>
        [Required]
        public Guid? GroupOrPinId
        {
            get => _groupOrPinId;
            set => SetProperty(ref _groupOrPinId, value);
        }

        /// <summary>
        /// 引脚或组名称
        /// </summary>
        public string PinOrGroupName { get; set; }

        /// <summary>
        /// 输入低电压
        /// </summary>
        [Required]
        public decimal? Vil
        {
            get => _vil;
            set => SetProperty(ref _vil, value);
        }

        /// <summary>
        /// 输入高电压
        /// </summary>
        [Required]
        public decimal? Vih
        {
            get => _vih;
            set => SetProperty(ref _vih, value);
        }

        /// <summary>
        /// 输出低电压
        /// </summary>
        [Required]
        public decimal? Vol
        {
            get => _vol;
            set => SetProperty(ref _vol, value);
        }

        /// <summary>
        /// 输出高电压
        /// </summary>
        [Required]
        public decimal? Voh
        {
            get => _voh;
            set => SetProperty(ref _voh, value);
        }

        /// <summary>
        /// 低电平输出灌电流
        /// </summary>
        [Required]
        public decimal? Iol
        {
            get => _iol;
            set => SetProperty(ref _iol, value);
        }

        /// <summary>
        /// 高电平输出拉电流
        /// </summary>
        [Required]
        public decimal? Ioh
        {
            get => _ioh;
            set => SetProperty(ref _ioh, value);
        }

        /// <summary>
        /// 电压基准
        /// </summary>
        [Required]
        public decimal? Vt
        {
            get => _vt;
            set => SetProperty(ref _vt, value);
        }

        /// <summary>
        /// 错位低电压
        /// </summary>
        [Required]
        public decimal? Vcl
        {
            get => _vcl;
            set => SetProperty(ref _vcl, value);
        }

        /// <summary>
        /// 错位高电压
        /// </summary>
        [Required]
        public decimal? Vch
        {
            get => _vch;
            set => SetProperty(ref _vch, value);
        }

        /// <summary>
        /// 供电电压
        /// </summary>
        public decimal? Ps
        {
            get => _ps;
            set => SetProperty(ref _ps, value);
        }

        /// <summary>
        /// 电流限制
        /// </summary>
        public decimal? I
        {
            get => _i;
            set => SetProperty(ref _i, value);
        }

        /// <summary>
        /// 上电延迟时间
        /// </summary>
        public int? Tdelay
        {
            get => _tdelay;
            set => SetProperty(ref _tdelay, value);
        }

        /// <summary>
        /// 上电顺序
        /// </summary>
        public int? Sequence
        {
            get => _sequence;
            set => SetProperty(ref _sequence, value);
        }

        /// <summary>
        /// 注释
        /// </summary>
        public string Comment
        {
            get => _comment;
            set => SetProperty(ref _comment, value);
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreationTime
        {
            get => _creationTime;
            set => SetProperty(ref _creationTime, value);
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? LastModificationTime
        {
            get => _lastModificationTime;
            set => SetProperty(ref _lastModificationTime, value);
        }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 版本号
        ///</summary>
        public byte[] Version { get; set; }

        /// <summary>
        /// 是否是新增
        /// </summary>
        public bool IsNew { get; set; }
    }
}
