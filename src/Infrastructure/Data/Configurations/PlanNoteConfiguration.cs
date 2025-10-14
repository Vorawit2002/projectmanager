using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Data.Configurations;
public class PlanNoteConfiguration : IEntityTypeConfiguration<PlanNote>
{
    public void Configure(EntityTypeBuilder<PlanNote> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasComment("รหัสรายการ");
        builder.Property(x => x.ActivityPlanId).IsRequired(true).HasComment("รหัสไอดีของการนัดหมาย");
        builder.Property(x => x.Summary).IsRequired(true).HasComment("สรุปรายงาน");
        builder.Property(x => x.ToDoNext).IsRequired(false).HasComment("สิ่งที่ต้องติดตามต่อ");
    }
}
