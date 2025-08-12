using KSW.ATE01.Domain.TestPlan.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KSW.ATE01.Sqlite.EntityTypeConfigurations.TestPlans
{
    public class LevelConfiguration : IEntityTypeConfiguration<Level>
    {
        public void Configure(EntityTypeBuilder<Level> builder)
        {
            ConfigTable(builder);
            ConfigId(builder);
            ConfigProperties(builder);
        }

        private void ConfigTable(EntityTypeBuilder<Level> builder)
        {
            ((EntityTypeBuilder)builder).ToTable($"Level", t => t.HasComment("测试项电平"));
        }

        private void ConfigId(EntityTypeBuilder<Level> builder)
        {
            builder.Property(t => t.Id)
                .HasColumnName("LevelId")
                .HasComment("测试项电平Id");
        }

        private void ConfigProperties(EntityTypeBuilder<Level> builder)
        {
            builder.Property(t => t.LevelGroupId)
                .HasColumnName("LevelGroupId")
                .HasComment("测试项电平组Id");
            builder.Property(t => t.GroupOrPinId)
                .HasColumnName("GroupOrPinId")
                .HasComment("组或引脚Id");
            builder.Property(t => t.Vil)
                .HasColumnName("Vil")
                .HasComment("输入低电压");
            builder.Property(t => t.Vih)
                .HasColumnName("Vih")
                .HasComment("输入高电压");
            builder.Property(t => t.Vol)
                .HasColumnName("Vol")
                .HasComment("输出低电压");
            builder.Property(t => t.Voh)
                .HasColumnName("Voh")
                .HasComment("输出高电压");
            builder.Property(t => t.Iol)
                .HasColumnName("Iol")
                .HasComment("低电平输出灌电流");
            builder.Property(t => t.Ioh)
                .HasColumnName("Ioh")
                .HasComment("高电平输出拉电流");
            builder.Property(t => t.Vt)
                .HasColumnName("Vt")
                .HasComment("电压基准");
            builder.Property(t => t.Vcl)
                .HasColumnName("Vcl")
                .HasComment("错位低电压");
            builder.Property(t => t.Vch)
                .HasColumnName("Vch")
                .HasComment("错位高电压");
            builder.Property(t => t.Ps)
                .HasColumnName("Ps")
                .HasComment("供电电压");
            builder.Property(t => t.I)
                .HasColumnName("I")
                .HasComment("电流限制");
            builder.Property(t => t.Tdelay)
                .HasColumnName("Tdelay")
                .HasComment("上电延迟时间");
            builder.Property(t => t.Sequence)
                .HasColumnName("Sequence")
                .HasComment("上电顺序");
            builder.Property(t => t.Comment)
                .HasColumnName("Comment")
                .HasComment("注释");
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
