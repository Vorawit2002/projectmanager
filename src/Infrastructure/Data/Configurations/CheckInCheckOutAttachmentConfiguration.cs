using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Data.Configurations;
public class CheckInCheckOutAttachmentConfiguration : IEntityTypeConfiguration<CheckInCheckOutAttachment>
{
public void Configure(EntityTypeBuilder<CheckInCheckOutAttachment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasComment("รหัสรายการ");
        builder.Property(x => x.CheckInCheckOutId).IsRequired(true).HasComment("รหัสไอดีของการนัดหมาย");
        builder.Property(x => x.AttachmentId).IsRequired(true).HasComment("รหัสไอดีของไฟล์");

    }
}


