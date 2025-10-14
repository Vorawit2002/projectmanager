using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Data.Configurations;
public class ActivityPlanContactConfiguration : IEntityTypeConfiguration<ActivityPlanContact>
{
public void Configure(EntityTypeBuilder<ActivityPlanContact> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasComment("รหัสรายการ");
        builder.Property(x => x.ActivityPlanId).IsRequired(true).HasComment("รหัสไอดีของการนัดหมาย");
        builder.Property(x => x.OrganizationContactId).IsRequired(true).HasComment("รหัสไอดีหน่วยงาน");

    }
}


