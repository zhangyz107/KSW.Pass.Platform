using KSW.ATE01.Domain.TestPlan.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Sqlite.EntityTypeConfigurations.TestPlans
{
    public class PinSiteInfoConfiguration : IEntityTypeConfiguration<PinSiteInfo>
    {
        public void Configure(EntityTypeBuilder<PinSiteInfo> builder)
        {
            ConfigTable(builder);
            ConfigId(builder);
            ConfigProperties(builder);
        }

        private void ConfigTable(EntityTypeBuilder<PinSiteInfo> builder)
        {
            ((EntityTypeBuilder)builder).ToTable($"PinSiteInfo", t => t.HasComment("引脚站点信息"));
        }

        private void ConfigId(EntityTypeBuilder<PinSiteInfo> builder)
        {
            builder.Property(t => t.Id)
                .HasColumnName("PinSiteInfoId")
                .HasComment("引脚站点信息Id");
        }

        private void ConfigProperties(EntityTypeBuilder<PinSiteInfo> builder)
        {
            builder.Property(t => t.SiteInfoId)
                .HasColumnName("SiteInfoId")
                .HasComment("站点信息Id");
            builder.Property(t => t.PinInfoId)
                .HasColumnName("PinInfoId")
                .HasComment("引脚信息Id");
            builder.Property(t => t.ChannelName)
                .HasColumnName("ChannelName")
                .HasComment("通道名称");
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
