using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace KSW.ATE01.Application.Models.RealTimeTxt
{
    /// <summary>
    /// 关键字模型
    /// </summary>
    public class KeywordModel : DtoBase
    {
        private int _sortId;
        private string _keyword;
        private Color? _foreground;
        private bool? _isHighlight;
        private DateTime? _createTime;

        /// <summary>
        /// 排序
        /// </summary>
        public int SortId
        {
            get => _sortId;
            set => SetProperty(ref _sortId, value);
        }

        /// <summary>
        /// 关键字
        /// </summary>
        public string Keyword
        {
            get => _keyword;
            set => SetProperty(ref _keyword, value);
        }

        /// <summary>
        /// 字体颜色
        /// </summary>
        public Color? Foreground
        {
            get => _foreground;
            set
            {
                if (SetProperty(ref _foreground, value))
                {
                    RaisePropertyChanged(nameof(ForegroundString));
                    RaisePropertyChanged(nameof(ForegroundBrush));
                }
            }
        }

        /// <summary>
        /// 字体颜色字符串
        /// </summary>
        public string ForegroundString=> $"#{_foreground?.R:X2}{_foreground?.G:X2}{_foreground?.B:X2}";

        /// <summary>
        /// 字体填充色
        /// </summary>
        public Brush ForegroundBrush => new SolidColorBrush(_foreground ?? Colors.Transparent);

        /// <summary>
        /// 是否高亮
        /// </summary>
        public bool? IsHighlight
        {
            get => _isHighlight;
            set => SetProperty(ref _isHighlight, value);
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime
        {
            get => _createTime;
            set => SetProperty(ref _createTime, value);
        }
    }
}
