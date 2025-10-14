using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Data.Configurations;
public class ProjectContactConfiguration : IEntityTypeConfiguration<ProjectContact>
{
    public void Configure(EntityTypeBuilder<ProjectContact> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasComment("รหัสรายการ");
        builder.Property(x => x.ProjectId).IsRequired(true).HasComment("รหัสโปรเจค");
        builder.Property(x => x.OrganizationContactId).IsRequired(true).HasComment("รหัสไอดีของผู้ติดต่อ");
       
    }
}
