using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Data.Configurations;
public class CheckInCheckOutConfiguration : IEntityTypeConfiguration<CheckInCheckOut>
{
public void Configure(EntityTypeBuilder<CheckInCheckOut> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasComment("รหัสรายการ");
        builder.Property(x => x.EmployeeId).IsRequired(true).HasComment("รหัสไอดีพนักงาน");
        builder.Property(x => x.Location).IsRequired(false).HasComment("สถานที่่");
        builder.Property(x => x.LocationCheckOut).IsRequired(false).HasComment("สถานที่ออกงาน");
        builder.Property(x => x.IPAddress).IsRequired(false).HasComment("ip เครื่อง");
        builder.Property(x => x.CheckIn).IsRequired(true).HasComment("เข้างาน");
        builder.Property(x => x.CheckOut).IsRequired(false).HasComment("ออกงาน");
        builder.Property(x => x.OrganizationId).IsRequired(false).HasComment("รหัสไอดีโปรเจค");
        builder.Property(x => x.ProjectId).IsRequired(false).HasComment("รหัสไอดีโปรเจค");
        
        builder
        .HasMany(e => e.CheckInCheckOutAttachments)
        .WithOne(s => s.CheckInCheckOuts) // ✅ ต้องตรงกับชื่อ property ที่แก้ในข้อ 1
        .HasForeignKey(s => s.CheckInCheckOutId) // ✅ ต้องเป็นชื่อของ foreign key ที่ถูกต้อง
        .OnDelete(DeleteBehavior.Cascade)
        .IsRequired(false);
    }

}


