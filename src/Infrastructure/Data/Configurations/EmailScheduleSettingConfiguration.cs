using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Infrastructure.Data.Configurations;
public class EmailScheduleSettingConfiguration : IEntityTypeConfiguration<EmailScheduleSetting>
{
    public void Configure(EntityTypeBuilder<EmailScheduleSetting> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.EmailMessageSetting).WithMany().HasForeignKey(x => x.EmailMessageSettingId);

        builder.Property(x => x.ScheduleName).HasMaxLength(int.MaxValue).HasComment("ชื่อการกำหนดเวลา");
        builder.Property(x => x.SendMailDate).HasComment("กำหนดวันที่ส่ง");
        builder.Property(x => x.IsEnabled).HasComment("สถานะเปิดใช้งาน");
        builder.Property(x => x.Id).HasComment("รหัสรายการ");
        builder.Property(x => x.Created).HasComment("วันที่สร้าง");
        builder.Property(x => x.CreatedBy).HasComment("ชื่อผู้สร้าง");
        builder.Property(x => x.LastModified).HasComment("วันที่ปรับปรุง");
        builder.Property(x => x.LastModifiedBy).HasComment("ชื่อผู้ปรับปรุง");
        builder.Property(x => x.DeletedBy).HasComment("ชื่อผู้ลบข้อมูล");
    }
}
