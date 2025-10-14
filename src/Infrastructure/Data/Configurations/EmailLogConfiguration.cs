using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Data.Configurations;
public class EmailLogConfiguration : IEntityTypeConfiguration<EmailLog>
{
    public void Configure(EntityTypeBuilder<EmailLog> builder)
    {
        builder.HasKey(x => x.Id);


        builder.Property(x => x.Id).HasComment("รหัสรายการ");
        builder.Property(x => x.Created).HasComment("วันที่สร้าง");
        builder.Property(x => x.CreatedBy).HasComment("ชื่อผู้สร้าง");
        builder.Property(x => x.LastModified).HasComment("วันที่ปรับปรุง");
        builder.Property(x => x.LastModifiedBy).HasComment("ชื่อผู้ปรับปรุง");
        builder.Property(x => x.DeletedBy).HasComment("ชื่อผู้ลบข้อมูล");
    }
}
