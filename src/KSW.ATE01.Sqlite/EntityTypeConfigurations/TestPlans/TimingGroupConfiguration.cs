using KSW.ATE01.Domain.TestPlan.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KSW.ATE01.Sqlite.EntityTypeConfigurations.TestPlans
{
    public class TimingGroupConfiguration : IEntityTypeConfiguration<TimingGroup>
    {
        public void Configure(EntityTypeBuilder<TimingGroup> builder)
        {
            ConfigTable(builder);
            ConfigId(builder);
            ConfigProperties(builder);
        }

        private void ConfigTable(EntityTypeBuilder<TimingGroup> builder)
        {
            ((EntityTypeBuilder)builder).ToTable($"TimingGroup", t => t.HasComment("测试项时钟组"));
        }

        private void ConfigId(EntityTypeBuilder<TimingGroup> builder)
        {
            builder.Property(t => t.Id)
                .HasColumnName("TimingGroupId")
                .HasComment("测试项时钟组Id");
        }

        private void ConfigProperties(EntityTypeBuilder<TimingGroup> builder)
        {
            builder.Property(t => t.ProjectInfoId)
                .HasColumnName("ProjectInfoId")
                .HasComment("项目信息Id");
            builder.Property(t => t.TimingGroupName)
                .HasColumnName("TimingGroupName")
                .HasComment("时钟组名");
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
