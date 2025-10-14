using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Data.Configurations;
public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasComment("รหัสรายการ");
        builder.Property(x => x.NameFile).IsRequired(true).HasComment("ชื่อไฟล์");
        builder.Property(x => x.PathFile).IsRequired(true).HasComment("ที่อยู่ของไฟล์");
        builder.Property(x => x.FileSize).IsRequired(true).HasComment("ขนาดไฟล์");
        builder.Property(x => x.FileExtension).IsRequired(true).HasComment("นามสกุลไฟล์");
        builder.Property(x => x.BucketOriginalName).IsRequired(true).HasComment("ชื่อที่เก็บไฟล์");
        builder.Property(x => x.BucketOriginalPath).IsRequired(true).HasComment("ที่อยู่ที่เก็บไฟล์");
    }
}


