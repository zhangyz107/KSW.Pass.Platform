using AutoMapper;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Domain.Projects.Entities;
using KSW.ObjectMapping.AutoMapper;

namespace KSW.ATE01.Application.ObjectMapping
{
    public class ProjectInfoMapConfig : IAutoMapperConfig
    {
        public void Config(IMapperConfigurationExpression expression)
        {
            expression.CreateMap<ProjectInfo, ProjectInfoModel>();
        }
    }
}
