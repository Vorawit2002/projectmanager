using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Data.Configurations;
public class OrganizationContactConfiguration : IEntityTypeConfiguration<OrganizationContact>
{
public void Configure(EntityTypeBuilder<OrganizationContact> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasComment("รหัสรายการ");
        builder.Property(x => x.OrganizationId).IsRequired(true).HasComment("รหัสไอดีของหน่วยงาน");
        builder.Property(x => x.TitleName).IsRequired(false).HasComment("คำนำหน้าชื่อ");
        builder.Property(x => x.FirstName).IsRequired(true).HasComment("ชื่อ");
        builder.Property(x => x.LastName).IsRequired(false).HasComment("นามสกุล");
        builder.Property(x => x.Position).IsRequired(false).HasComment("ตำแหน่ง");
        builder.Property(x => x.Email).IsRequired(false).HasComment("อีเมล");
        builder.Property(x => x.Phone).IsRequired(false).HasComment("เบอร์โทร");
        builder.Property(x => x.Fax).IsRequired(false).HasComment("แฟกซ์");
        builder.Property(x => x.LineId).IsRequired(false).HasComment("ไลน์ไอดี");

    }
}


