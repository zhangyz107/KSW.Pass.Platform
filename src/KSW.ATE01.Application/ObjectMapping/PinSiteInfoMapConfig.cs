using AutoMapper;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ObjectMapping.AutoMapper;

namespace KSW.ATE01.Application.ObjectMapping
{
    public class PinSiteInfoMapConfig : IAutoMapperConfig
    {
        public void Config(IMapperConfigurationExpression expression)
        {
            expression.CreateMap<PinSiteInfo, PinSiteInfoModel>();
        }
    }
}
