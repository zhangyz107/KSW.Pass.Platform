using AutoMapper;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Project.Base.Models.TestPlans;
using KSW.ObjectMapping.AutoMapper;

namespace KSW.ATE01.Application.ObjectMapping
{
    public class FlowInfoMapConfig : IAutoMapperConfig
    {
        public void Config(IMapperConfigurationExpression expression)
        {
            expression.CreateMap<FlowModel, FlowInfoModel>();
        }
    }
}
