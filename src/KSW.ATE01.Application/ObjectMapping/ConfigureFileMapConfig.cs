using AutoMapper;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Application.Models.RealTimeTxt;
using KSW.ATE01.Domain.Projects.Entities;
using KSW.ATE01.Domain.RealTimeTxt.Entities;
using KSW.ObjectMapping.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.ObjectMapping
{
    public class ConfigureFileMapConfig : IAutoMapperConfig
    {
        public void Config(IMapperConfigurationExpression expression)
        {
            expression.CreateMap<ConfigureFile, ConfigureFileModel>();
            expression.CreateMap<ConfigureFile, ConfigureFileModel>().ReverseMap();
        }
    }
}
