using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Models.RealTimeTxt
{
    /// <summary>
    /// 配置文件模型
    /// </summary>
    public class ConfigureFileModel : DtoBase
    {
        private string _fileEncoding;
        private int _fileChangeInterval;
        private int _fileReopenInterval;
        private List<KeywordModel> _keywords = new List<KeywordModel>();

        /// <summary>
        /// 文本编码
        /// </summary>
        public string FileEncoding
        {
            get => _fileEncoding;
            set => SetProperty(ref _fileEncoding, value);
        }

        /// <summary>
        /// 文件更改间隔
        /// </summary>
        public int FileChangeInterval
        {
            get => _fileChangeInterval;
            set
            {
                if (value > 0)
                {
                    SetProperty(ref _fileChangeInterval, value);
                }
            }
        }

        /// <summary>
        /// 文件重新打开间隔
        /// </summary>
        public int FileReopenInterval
        {
            get => _fileReopenInterval;
            set
            {
                if (value > 0)
                {
                    SetProperty(ref _fileReopenInterval, value);
                }
            }

        }

        /// <summary>
        /// 关键字集合
        /// </summary>
        public List<KeywordModel> Keywords
        {
            get => _keywords;
            set => SetProperty(ref _keywords, value);
        }
    }
}
