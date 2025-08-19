using KSW.ATE01.Domain.TestPlan.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KSW.ATE01.Sqlite.EntityTypeConfigurations.TestPlans
{
    public class GlobalParameterConfiguration : IEntityTypeConfiguration<GlobalParameter>
    {
        public void Configure(EntityTypeBuilder<GlobalParameter> builder)
        {
            ConfigTable(builder);
            ConfigId(builder);
            ConfigProperties(builder);
        }

        /// <summary>
        /// 配置表
        /// </summary>
        private void ConfigTable(EntityTypeBuilder<GlobalParameter> builder)
        {
            ((EntityTypeBuilder)builder).ToTable($"GlobalParameter", t => t.HasComment("全局参数"));
        }

        /// <summary>
        /// 配置标识
        /// </summary>
        private void ConfigId(EntityTypeBuilder<GlobalParameter> builder)
        {
            builder.Property(t => t.Id)
                .HasColumnName("GlobalParameterId")
                .HasComment("全局参数Id");
        }

        /// <summary>
        /// 配置属性
        /// </summary>
        private void ConfigProperties(EntityTypeBuilder<GlobalParameter> builder)
        {
            builder.Property(t => t.ProjectInfoId)
                .HasColumnName("ProjectInfoId")
                .HasComment("项目信息Id");
            builder.Property(t => t.PatternFile)
                .HasColumnName("PatternFile")
                .HasComment("向量文件名称");
            builder.Property(t => t.AdditionInfo)
                .HasColumnName("AdditionInfo")
                .HasComment("附加信息");
            builder.Property(t => t.CreationTime)
                .HasColumnName("CreationTime")
                .HasComment("创建时间");
            builder.Property(t => t.LastModificationTime)
                .HasColumnName("LastModificationTime")
                .HasComment("最后修改时间");
            builder.Property(t => t.IsDeleted)
                .HasColumnName("IsDeleted")
                .HasComment("是否删除");
        }
    }
}
