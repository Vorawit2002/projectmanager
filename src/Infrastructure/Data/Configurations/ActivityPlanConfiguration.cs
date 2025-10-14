using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Data.Configurations;
public class ActivityPlanConfiguration : IEntityTypeConfiguration<ActivityPlan>
{
public void Configure(EntityTypeBuilder<ActivityPlan> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasComment("รหัสรายการ");
        builder.Property(x => x.EmployeeId).IsRequired(true).HasComment("รหัสไอดีของพนักงาน");
        builder.Property(x => x.Objective).IsRequired(false).HasComment("วัตถุประสงค์");
        builder.Property(x => x.ObjectiveDetail).IsRequired(false).HasComment("รายละเอียดวัตถุประสงค์");
        builder.Property(x => x.detail).IsRequired(false).HasComment("รายละเอียด");
        builder.Property(x => x.ProjectId).IsRequired(false).HasComment("รหัสไอดีโปรเจค");
        builder.Property(x => x.OrganizationId).IsRequired(false).HasComment("รหัสไอดีหน่วยงาน");
        //builder.Property(x => x.ActivityPlanStatus).IsRequired(false).HasComment("สถานะของกิจกรรม");
        builder.Property(x => x.AllDay).IsRequired(false).HasComment("ทั้งวันหรือไม่");
        builder.Property(x => x.StartDate).IsRequired(true).HasComment("วันเวลาที่เริ่มต้น");
        builder.Property(x => x.EndDate).IsRequired(true).HasComment("วันเวลาที่สิ้นสุด");
        builder.Property(x => x.Location).IsRequired(false).HasComment("สถานที่");
        builder.Property(x => x.HaveCost).IsRequired(false).HasComment("มี/ไม่มีค่าใช้จ่าย");
        builder.Property(x => x.CostDetail).IsRequired(false).HasComment("รายละเอียดของค่าใช้จ่าย");
        builder.Property(x => x.Cost).IsRequired(false).HasComment("ค่าใช้จ่าย");
        builder.Property(x => x.EventTypeId).IsRequired(false).HasComment("รหัสไอดีประเภทกิจกรรม");

        builder
    .HasMany(e => e.ActivityPlanContacts)
    .WithOne(s => s.ActivityPlan) // ✅ ต้องตรงกับชื่อ property ที่แก้ในข้อ 1
    .HasForeignKey(s => s.ActivityPlanId) // ✅ ต้องเป็นชื่อของ foreign key ที่ถูกต้อง
    .OnDelete(DeleteBehavior.Cascade)
     .IsRequired(false);

        builder
    .HasMany(e => e.PlanNotes)
    .WithOne(s => s.ActivityPlans) // ✅ ต้องตรงกับชื่อ property ที่แก้ในข้อ 1
    .HasForeignKey(s => s.ActivityPlanId) // ✅ ต้องเป็นชื่อของ foreign key ที่ถูกต้อง
    .OnDelete(DeleteBehavior.Cascade)
     .IsRequired(false);

        builder
 .HasMany(e => e.ActivityPlanAttachments)
 .WithOne(s => s.ActivityPlans) // ✅ ต้องตรงกับชื่อ property ที่แก้ในข้อ 1
 .HasForeignKey(s => s.ActivityPlanId) // ✅ ต้องเป็นชื่อของ foreign key ที่ถูกต้อง
 .OnDelete(DeleteBehavior.Cascade)
  .IsRequired(false);
    }
}


