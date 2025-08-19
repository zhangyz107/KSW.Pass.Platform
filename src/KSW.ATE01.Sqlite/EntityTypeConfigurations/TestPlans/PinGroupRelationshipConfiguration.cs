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
    public class PinGroupRelationshipConfiguration : IEntityTypeConfiguration<PinGroupRelationship>
    {
        public void Configure(EntityTypeBuilder<PinGroupRelationship> builder)
        {
            ConfigTable(builder);
            ConfigId(builder);
            ConfigProperties(builder);
        }

        /// <summary>
        /// 配置表
        /// </summary>
        private void ConfigTable(EntityTypeBuilder<PinGroupRelationship> builder)
        {
            ((EntityTypeBuilder)builder).ToTable($"PinGroupRelationship", t => t.HasComment("引脚与组关系"));
        }

        private void ConfigId(EntityTypeBuilder<PinGroupRelationship> builder)
        {
            builder.Property(t => t.Id)
                .HasColumnName("PinGroupRelationshipId")
                .HasComment("引脚与组关系Id");
        }

        private void ConfigProperties(EntityTypeBuilder<PinGroupRelationship> builder)
        {
            builder.Property(t => t.PinInfoId)
                 .HasColumnName("PinInfoId")
                 .HasComment("引脚信息Id");
            builder.Property(t => t.GroupInfoId)
                 .HasColumnName("GroupInfoId")
                 .HasComment("组信息Id");
            builder.Property(t => t.CreationTime)
                .HasColumnName("CreationTime")
                .HasComment("创建时间");
            builder.Property(t => t.LastModificationTime)
                .HasColumnName("LastModificationTime")
                .HasComment("最后修改时间");
        }
    }
}
