using KSW.ATE01.Domain.TestPlan.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KSW.ATE01.Sqlite.EntityTypeConfigurations.TestPlans
{
    public class PinInfoConfiguration : IEntityTypeConfiguration<PinInfo>
    {
        public void Configure(EntityTypeBuilder<PinInfo> builder)
        {
            ConfigTable(builder);
            ConfigId(builder);
            ConfigProperties(builder);
        }

        /// <summary>
        /// 配置表
        /// </summary>
        private void ConfigTable(EntityTypeBuilder<PinInfo> builder)
        {
            ((EntityTypeBuilder)builder).ToTable($"PinInfo", t => t.HasComment("项目信息"));
        }

        /// <summary>
        /// 配置标识
        /// </summary>
        private void ConfigId(EntityTypeBuilder<PinInfo> builder)
        {
            builder.Property(t => t.Id)
                .HasColumnName("PinInfoId")
                .HasComment("引脚信息Id");
        }

        /// <summary>
        /// 配置属性
        /// </summary>
        private void ConfigProperties(EntityTypeBuilder<PinInfo> builder)
        {
            builder.Property(t => t.PinOverviewId)
                .HasColumnName("PinOverviewId")
                .HasComment("引脚总览Id");
            builder.Property(t => t.PinName)
                .HasColumnName("PinName")
                .HasComment("引脚名称");
            builder.Property(t => t.PinType)
                .HasColumnName("PinType")
                .HasComment("引脚类型");
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
