using KSW.ATE01.Domain.TestPlan.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KSW.ATE01.Sqlite.EntityTypeConfigurations.TestPlans
{
    public class GroupInfoConfiguration : IEntityTypeConfiguration<GroupInfo>
    {
        public void Configure(EntityTypeBuilder<GroupInfo> builder)
        {
            ConfigTable(builder);
            ConfigId(builder);
            ConfigProperties(builder);
        }

        /// <summary>
        /// 配置表
        /// </summary>
        private void ConfigTable(EntityTypeBuilder<GroupInfo> builder)
        {
            ((EntityTypeBuilder)builder).ToTable($"GroupInfo", t => t.HasComment("组信息"));
        }

        /// <summary>
        /// 配置标识
        /// </summary>
        private void ConfigId(EntityTypeBuilder<GroupInfo> builder)
        {
            builder.Property(t => t.Id)
                .HasColumnName("GroupInfoId")
                .HasComment("组信息Id");
        }

        /// <summary>
        /// 配置属性
        /// </summary>
        private void ConfigProperties(EntityTypeBuilder<GroupInfo> builder)
        {
            builder.Property(t => t.PinOverviewId)
                .HasColumnName("PinOverviewId")
                .HasComment("引脚总览Id");
            builder.Property(t => t.GroupName)
                .HasColumnName("GroupName")
                .HasComment("组名称");
            builder.Property(t => t.CreateTime)
                .HasColumnName("CreateTime")
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
