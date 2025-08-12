using KSW.ATE01.Domain.TestPlan.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KSW.ATE01.Sqlite.EntityTypeConfigurations.TestPlans
{
    public class LimitsConfiguration : IEntityTypeConfiguration<Limits>
    {
        public void Configure(EntityTypeBuilder<Limits> builder)
        {
            ConfigTable(builder);
            ConfigId(builder);
            ConfigProperties(builder);
        }

        /// <summary>
        /// 配置表
        /// </summary>
        private void ConfigTable(EntityTypeBuilder<Limits> builder)
        {
            ((EntityTypeBuilder)builder).ToTable($"Limits", t => t.HasComment("测试项门限"));
        }

        /// <summary>
        /// 配置标识
        /// </summary>
        private void ConfigId(EntityTypeBuilder<Limits> builder)
        {
            builder.Property(t => t.Id)
                .HasColumnName("LimitsId")
                .HasComment("测试项门限Id");
        }

        /// <summary>
        /// 配置属性
        /// </summary>
        private void ConfigProperties(EntityTypeBuilder<Limits> builder)
        {
            builder.Property(t => t.TestItemInfoId)
                .HasColumnName("TestItemInfoId")
                .HasComment("测试项信息Id");
            builder.Property(t => t.TestNumber)
                .HasColumnName("TestNumber")
                .HasComment("测试编号");
            builder.Property(t => t.LowLimit)
                .HasColumnName("LowLimit")
                .HasComment("电压下限");
            builder.Property(t => t.HighLimit)
                .HasColumnName("HighLimit")
                .HasComment("电压上限");
            builder.Property(t => t.Units)
                .HasColumnName("Units")
                .HasComment("单位");
            builder.Property(t => t.LimitName)
                .HasColumnName("LimitName")
                .HasComment("门限名称");
            builder.Property(t => t.FailSoftwareBin)
                .HasColumnName("FailSoftwareBin")
                .HasComment("软件失效分档");
            builder.Property(t => t.PassSoftwareBin)
                .HasColumnName("PassSoftwareBin")
                .HasComment("软件成功分档");
            builder.Property(t => t.FailHardwareBin)
                .HasColumnName("FailHardwareBin")
                .HasComment("硬件失效分档");
            builder.Property(t => t.PassHardwareBin)
                .HasColumnName("PassHardwareBin")
                .HasComment("硬件成功分档");
            builder.Property(t => t.DutResult)
                .HasColumnName("DutResult")
                .HasComment("被测物结果");
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
