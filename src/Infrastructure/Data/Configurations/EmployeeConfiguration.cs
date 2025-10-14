using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Data.Configurations;
public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasComment("รหัสรายการ");
        builder.Property(x => x.UserId).IsRequired(true).HasComment("รหัสไอดีจากOpenid");
        builder.Property(x => x.TitleName).IsRequired(false).HasComment("คำนำหน้าชื่อ");
        builder.Property(x => x.FirstName).IsRequired(false).HasComment("ชื่อ");
        builder.Property(x => x.LastName).IsRequired(false).HasComment("นามสกุล");
        builder.Property(x => x.Email).IsRequired(true).HasComment("อีเมล");
        builder.Property(x => x.Position).IsRequired(false).HasComment("ตำแหน่ง");
        builder.Property(x => x.Phone).IsRequired(false).HasComment("เบอร์โทรศัพท์");
        builder.Property(x => x.DepartmentId).IsRequired(false).HasComment("รหัสไอดีของแผนก");

    }
}


