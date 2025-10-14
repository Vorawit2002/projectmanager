using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Data.Configurations;
public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasComment("รหัสรายการ");
        builder.Property(x => x.Name).IsRequired(true).HasComment("ชื่อแผนก");
        builder.Property(x => x.IsActive).IsRequired(true).HasComment("การใช้งาน");

    }
}


