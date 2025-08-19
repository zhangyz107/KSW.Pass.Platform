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
    public class LevelGroupConfiguration : IEntityTypeConfiguration<LevelGroup>
    {
        public void Configure(EntityTypeBuilder<LevelGroup> builder)
        {
            ConfigTable(builder);
            ConfigId(builder);
            ConfigProperties(builder);
        }

        /// <summary>
        /// 配置表
        /// </summary>
        private void ConfigTable(EntityTypeBuilder<LevelGroup> builder)
        {
            ((EntityTypeBuilder)builder).ToTable($"LevelGroup", t => t.HasComment("测试项电平组"));
        }

        /// <summary>
        /// 配置标识
        /// </summary>
        private void ConfigId(EntityTypeBuilder<LevelGroup> builder)
        {
            builder.Property(t => t.Id)
                .HasColumnName("LevelGroupId")
                .HasComment("测试项电平组Id");
        }

        /// <summary>
        /// 配置属性
        /// </summary>
        private void ConfigProperties(EntityTypeBuilder<LevelGroup> builder)
        {
            builder.Property(t => t.ProjectInfoId)
                .HasColumnName("ProjectInfoId")
                .HasComment("项目信息Id");
            builder.Property(t => t.LevelGroupName)
                .HasColumnName("LevelGroupName")
                .HasComment("电平组名");
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
