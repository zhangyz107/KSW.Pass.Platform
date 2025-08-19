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
    public class SiteInfoConfiguration : IEntityTypeConfiguration<SiteInfo>
    {
        public void Configure(EntityTypeBuilder<SiteInfo> builder)
        {
            ConfigTable(builder);
            ConfigId(builder);
            ConfigProperties(builder);
        }

        /// <summary>
        /// 配置表
        /// </summary>
        private void ConfigTable(EntityTypeBuilder<SiteInfo> builder)
        {
            ((EntityTypeBuilder)builder).ToTable($"SiteInfo", t => t.HasComment("站点信息"));
        }

        /// <summary>
        /// 配置标识
        /// </summary>
        private void ConfigId(EntityTypeBuilder<SiteInfo> builder)
        {
            builder.Property(t => t.Id)
                .HasColumnName("SiteInfoId")
                .HasComment("站点信息Id");
        }

        /// <summary>
        /// 配置属性
        /// </summary>
        private void ConfigProperties(EntityTypeBuilder<SiteInfo> builder)
        {
            builder.Property(t => t.PinOverviewId)
                .HasColumnName("PinOverviewId")
                .HasComment("引脚总览Id"); 
            builder.Property(t => t.SortId)
                .HasColumnName("SortId")
                .HasComment("序号");
            builder.Property(t => t.SiteName)
                .HasColumnName("SiteName")
                .HasComment("站点名称");
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
