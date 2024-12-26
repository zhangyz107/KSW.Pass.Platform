/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：ConfigureFileBLL.cs
// 功能描述：文件配置逻辑层
//
// 作者：zhangyingzhong
// 日期：2024/12/25 18:25
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.Application;
using KSW.ATE01.Application.BLLs.Abstractions.RealTimeTxt;
using KSW.ATE01.Application.Models.RealTimeTxt;
using KSW.ATE01.Domain.RealTimeTxt.Entities;
using KSW.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.BLLs.Implements.RealTimeTxt
{
    /// <summary>
    /// 文件配置逻辑层
    /// </summary>
    public class ConfigureFileBLL : ServiceBase, IConfigureFileBLL
    {
        private ConfigureFileModel _configureFile;
        private readonly string _filePath;

        public ConfigureFileBLL(IContainerProvider containerProvider) : base(containerProvider)
        {
            var dir = ConfigurationManager.AppSettings["RealTimeDir"];
            var fileName = ConfigurationManager.AppSettings["RealTimeConfigureName"];
            _filePath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), dir, fileName);
        }

        public ConfigureFileModel GetConfigureFile()
        {
            return _configureFile ?? LoadConfigureFileFromDisk();
        }


        public void SaveConfigureFile(ConfigureFileModel configureFile)
        {
            _configureFile = configureFile;
        }

        public void SaveConfigureFileToDisk(ConfigureFileModel configureFile)
        {
            var entity = configureFile.MapTo<ConfigureFile>();
            var dir = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            XmlHelper.SerializeToXml(entity, _filePath);
            _configureFile = configureFile;
        }

        private ConfigureFileModel LoadConfigureFileFromDisk()
        {
            if (File.Exists(_filePath))
            {
                var configureFile = XmlHelper.DeserializeFromXml<ConfigureFile>(_filePath);
                _configureFile = configureFile?.MapTo<ConfigureFileModel>();
                if (_configureFile != null && !_configureFile.Keywords.IsEmpty())
                {
                    _configureFile.Keywords.OrderBy(x => x.CreateTime);
                    int sortId = 0;
                    foreach (var keyword in _configureFile.Keywords)
                    {
                        keyword.SortId = ++sortId;
                    }
                }
            }

            if(_configureFile == null)
                _configureFile = (new ConfigureFile()).MapTo<ConfigureFileModel>();

            return _configureFile;
        }
    }
}
