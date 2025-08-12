using KSW.ATE01.Domain.Projects.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KSW.ATE01.Sqlite.EntityTypeConfigurations.Projects
{
    public class ProjectInfoConfiguration : IEntityTypeConfiguration<ProjectInfo>
    {
        public void Configure(EntityTypeBuilder<ProjectInfo> builder)
        {
            ConfigTable(builder);
            ConfigId(builder);
            ConfigProperties(builder);
        }

        /// <summary>
        /// 配置表
        /// </summary>
        private void ConfigTable(EntityTypeBuilder<ProjectInfo> builder)
        {
            ((EntityTypeBuilder)builder).ToTable($"ProjectInfo", t => t.HasComment("项目信息"));
        }

        /// <summary>
        /// 配置标识
        /// </summary>
        private void ConfigId(EntityTypeBuilder<ProjectInfo> builder)
        {
            builder.Property(t => t.Id)
                .HasColumnName("ProjectInfoId")
                .HasComment("项目信息Id");
        }

        /// <summary>
        /// 配置属性
        /// </summary>
        private void ConfigProperties(EntityTypeBuilder<ProjectInfo> builder)
        {
            builder.Property(t => t.ProjectName)
                .HasColumnName("ProjectName")
                .HasComment("项目名称");
            builder.Property(t => t.ProjectPath)
                .HasColumnName("ProjectPath")
                .HasComment("项目路径");
            builder.Property(t => t.ProjectVersion)
                .HasColumnName("ProjectVersion")
                .HasComment("项目版本");
            builder.Property(t => t.SaveRealTimeText)
                .HasColumnName("SaveRealTimeText")
                .HasComment("记录RealTimeTxT");
            builder.Property(t => t.SaveCsv)
                .HasColumnName("SaveCsv")
                .HasComment("记录CSV");
            builder.Property(t => t.SaveSummary)
                .HasColumnName("SaveSummary")
                .HasComment("记录Summary");
            builder.Property(t => t.SaveStdf)
                .HasColumnName("SaveStdf")
                .HasComment("记录STDF");
            builder.Property(t => t.DatalogPath)
                .HasColumnName("DatalogPath")
                .HasComment("日志路径");
            builder.Property(t => t.IsDoAll)
                .HasColumnName("IsDoAll")
                .HasComment("是否DoAll");
            builder.Property(t => t.IsPrintTime)
                .HasColumnName("IsPrintTime")
                .HasComment("是否打印时间");
            builder.Property(t => t.LoopCount)
                .HasColumnName("LoopCount")
                .HasComment("循环次数");
            builder.Property(t => t.DelayBetweenLoops)
                .HasColumnName("DelayBetweenLoops")
                .HasComment("循环间时延");
            builder.Property(t => t.StopOnFail)
                .HasColumnName("StopOnFail")
                .HasComment("失败时停止");
            builder.Property(t => t.ReleasePath)
                .HasColumnName("ReleasePath")
                .HasComment("发布路径");
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
