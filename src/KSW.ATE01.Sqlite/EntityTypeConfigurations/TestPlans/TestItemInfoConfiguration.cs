using KSW.ATE01.Domain.TestPlan.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KSW.ATE01.Sqlite.EntityTypeConfigurations.TestPlans
{
    public class TestItemInfoConfiguration : IEntityTypeConfiguration<TestItemInfo>
    {
        public void Configure(EntityTypeBuilder<TestItemInfo> builder)
        {
            ConfigTable(builder);
            ConfigId(builder);
            ConfigProperties(builder);
        }

        private void ConfigTable(EntityTypeBuilder<TestItemInfo> builder)
        {
            ((EntityTypeBuilder)builder).ToTable($"TestItemInfo", t => t.HasComment("测试项信息"));
        }

        private void ConfigId(EntityTypeBuilder<TestItemInfo> builder)
        {
            builder.Property(t => t.Id)
                .HasColumnName("TestItemInfoId")
                .HasComment("测试项信息Id");
        }

        private void ConfigProperties(EntityTypeBuilder<TestItemInfo> builder)
        {
            builder.Property(t => t.ProjectInfoId)
                .HasColumnName("ProjectInfoId")
                .HasComment("项目信息Id");
            builder.Property(t => t.TestItemName)
                .HasColumnName("TestItemName")
                .HasComment("测试项名称");
            builder.Property(t => t.FunctionName)
                .HasColumnName("FunctionName")
                .HasComment("方法名称");
            builder.Property(t => t.Force)
                .HasColumnName("Force")
                .HasComment("激励");
            builder.Property(t => t.GroupOrPinId)
                .HasColumnName("GroupOrPinId")
                .HasComment("组或引脚Id");
            builder.Property(t => t.LimitsId)
                .HasColumnName("LimitsId")
                .HasComment("测试项门限Id");
            builder.Property(t => t.LevelGroupId)
                .HasColumnName("LevelGroupId")
                .HasComment("测试项电平组Id");
            builder.Property(t => t.TimingGroupId)
                .HasColumnName("TimingGroupId")
                .HasComment("测试项时钟组Id");
            builder.Property(t => t.AdditionInfo)
                .HasColumnName("AdditionInfo")
                .HasComment("附加信息");
            builder.Property(t => t.FlowIndex)
                .HasColumnName("FlowIndex")
                .HasComment("流程序号");
            builder.Property(t => t.Enable)
                .HasColumnName("Enable")
                .HasComment("是否启用");
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
