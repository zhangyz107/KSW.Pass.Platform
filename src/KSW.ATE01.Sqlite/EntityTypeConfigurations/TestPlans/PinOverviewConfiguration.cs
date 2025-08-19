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
    public class PinOverviewConfiguration : IEntityTypeConfiguration<PinOverview>
    {
        public void Configure(EntityTypeBuilder<PinOverview> builder)
        {
            ConfigTable(builder);
            ConfigId(builder);
            ConfigProperties(builder);
        }

        /// <summary>
        /// 配置表
        /// </summary>
        private void ConfigTable(EntityTypeBuilder<PinOverview> builder)
        {
            ((EntityTypeBuilder)builder).ToTable($"PinOverview", t => t.HasComment("引脚总览"));
        }

        /// <summary>
        /// 配置标识
        /// </summary>
        private void ConfigId(EntityTypeBuilder<PinOverview> builder)
        {
            builder.Property(t => t.Id)
                .HasColumnName("PinOverview")
                .HasComment("引脚总览Id");
        }

        /// <summary>
        /// 配置属性
        /// </summary>
        private void ConfigProperties(EntityTypeBuilder<PinOverview> builder)
        {
            builder.Property(t => t.ProjectInfoId)
                .HasColumnName("ProjectInfoId")
                .HasComment("项目信息Id");
            builder.Property(t => t.SiteCount)
                .HasColumnName("SiteCount")
                .HasComment("站点数");
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
