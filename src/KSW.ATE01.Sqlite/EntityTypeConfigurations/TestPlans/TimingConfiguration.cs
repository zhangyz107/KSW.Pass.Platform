using KSW.ATE01.Domain.TestPlan.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KSW.ATE01.Sqlite.EntityTypeConfigurations.TestPlans
{
    public class TimingConfiguration : IEntityTypeConfiguration<Timing>
    {
        public void Configure(EntityTypeBuilder<Timing> builder)
        {
            ConfigTable(builder);
            ConfigId(builder);
            ConfigProperties(builder);
        }

        /// <summary>
        /// 配置表
        /// </summary>
        private void ConfigTable(EntityTypeBuilder<Timing> builder)
        {
            ((EntityTypeBuilder)builder).ToTable($"Timing", t => t.HasComment("测试项时钟"));
        }

        /// <summary>
        /// 配置标识
        /// </summary>
        private void ConfigId(EntityTypeBuilder<Timing> builder)
        {
            builder.Property(t => t.Id)
                .HasColumnName("TimingId")
                .HasComment("测试项时钟Id");
        }

        /// <summary>
        /// 配置属性
        /// </summary>
        private void ConfigProperties(EntityTypeBuilder<Timing> builder)
        {
            builder.Property(t => t.TimingGroupId)
                .HasColumnName("TimingGroupId")
                .HasComment("测试项时钟组Id");
            builder.Property(t => t.TimingName)
                .HasColumnName("TimingName")
                .HasComment("时钟名称");
            builder.Property(t => t.Period)
                .HasColumnName("Period")
                .HasComment("周期");
            builder.Property(t => t.GroupOrPinId)
                .HasColumnName("GroupOrPinId")
                .HasComment("组或引脚Id");
            builder.Property(t => t.WaveformFormat)
                .HasColumnName("WaveformFormat")
                .HasComment("波形格式");
            builder.Property(t => t.DriveA)
                .HasColumnName("DriveA")
                .HasComment("环绕边缘");
            builder.Property(t => t.DriveB)
                .HasColumnName("DriveB")
                .HasComment("起始边缘");
            builder.Property(t => t.DriveC)
                .HasColumnName("DriveC")
                .HasComment("返回边缘");
            builder.Property(t => t.DriveD)
                .HasColumnName("DriveD")
                .HasComment("关闭边缘");
            builder.Property(t => t.StrobeMode)
                .HasColumnName("StrobeMode")
                .HasComment("选通模式");
            builder.Property(t => t.StrobeA)
                .HasColumnName("StrobeA")
                .HasComment("选通开始时间");
            builder.Property(t => t.StrobeB)
                .HasColumnName("StrobeB")
                .HasComment("选通结束时间");
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
