using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Data.Configurations;
public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasComment("รหัสรายการ");
        builder.Property(x => x.Name).IsRequired(true).HasComment("ชื่อหน่วยงาน");
        builder.Property(x => x.ShortName).IsRequired(false).HasComment("ชื่อย่อหน่วยงาน");
        builder.Property(x => x.TypeOrganization).IsRequired(false).HasComment("ประเภทหน่วยงาน");
        builder.Property(x => x.Address).IsRequired(false).HasComment("ที่อยู่");
        builder.Property(x => x.Coordinates).IsRequired(false).HasComment("พิกัด");
        builder.Property(x => x.WebSite).IsRequired(false).HasComment("เว็บไซต์");
        builder.Property(x => x.Phone).IsRequired(false).HasComment("เบอร์โทร");
        builder.Property(x => x.Fax).IsRequired(false).HasComment("แฟกซ์");

        builder
     .HasMany(e => e.OrganizationContacts)
     .WithOne(s => s.Organizations)
     .HasForeignKey(e => e.OrganizationId)
     .OnDelete(DeleteBehavior.Cascade)
     .IsRequired(false);

//        builder
//.HasMany(e => e.Projects)
//.WithOne(s => s.Organizations)
//.HasForeignKey(e => e.OrganizationId)
//.OnDelete(DeleteBehavior.Cascade)
//.IsRequired(false);
    }
}


