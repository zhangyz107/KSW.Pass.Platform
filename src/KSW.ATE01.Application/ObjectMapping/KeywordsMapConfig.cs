using AutoMapper;
using KSW.ATE01.Application.Models.RealTimeTxt;
using KSW.ATE01.Domain.RealTimeTxt.Entities;
using KSW.Helpers;
using KSW.ObjectMapping.AutoMapper;
using System.Collections.ObjectModel;

namespace KSW.ATE01.Application.ObjectMapping
{
    public class KeywordsMapConfig : IAutoMapperConfig
    {
        public void Config(IMapperConfigurationExpression expression)
        {
            expression.CreateMap<Keywords, KeywordModel>();
        }
    }
}
