using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Data.Configurations;
public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasComment("รหัสรายการ");
        builder.Property(x => x.ProjectCode).IsRequired(true).HasComment("รหัสโปรเจค");
        builder.Property(x => x.ProjectName).IsRequired(true).HasComment("ชื่อโปรเจค");
        builder.Property(x => x.ShortName).IsRequired(false).HasComment("ชื่อย่อโปรเจค");
        builder.Property(x => x.ContractNumber).IsRequired(false).HasComment("เลขที่สัญญา");
        builder.Property(x => x.ContractSignedDate).IsRequired(false).HasComment("วันที่ลงนามสัญญา");
        builder.Property(x => x.StartDate).IsRequired(false).HasComment("วันที่เริ่มงาน");
        builder.Property(x => x.EndDate).IsRequired(false).HasComment("วันที่สิ้นสุด");
        builder.Property(x => x.WarrantyEndDate).IsRequired(false).HasComment("วันที่สิ้นสุดรับประกันผลงาน");
        builder.Property(x => x.OrganizationId).IsRequired(false).HasComment("หน่วยงาน");
        builder.Property(x => x.ProjectCost).IsRequired(false).HasComment("งบประมาณโครงการ");
        builder.Property(x => x.ProjectType).IsRequired(false).HasComment("ประเภทโปรเจค");

//        builder
//.HasMany(e => e.ProjectContacts)
//.WithOne(s => s.Projects)
//.HasForeignKey(e => e.ProjectId)
//.OnDelete(DeleteBehavior.Cascade)
//.IsRequired(false);
    }
}
