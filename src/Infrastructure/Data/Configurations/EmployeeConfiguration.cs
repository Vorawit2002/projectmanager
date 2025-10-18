using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);

        // UserId is required and should be unique (1:1 with ApplicationUser)
        builder.Property(e => e.UserId)
            .IsRequired()
            .HasMaxLength(450); // Same as AspNetUsers.Id

        // Create unique index on UserId to enforce 1:1 relationship
        builder.HasIndex(e => e.UserId)
            .IsUnique();

        // Configure relationship with ApplicationUser
        builder.HasOne(e => e.User)
            .WithOne(u => u.Employee)
            .HasForeignKey<Employee>(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Delete employee when user is deleted

        // Email is required
        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(256);

        // String length constraints
        builder.Property(e => e.TitleName)
            .HasMaxLength(50);

        builder.Property(e => e.FirstName)
            .HasMaxLength(100);

        builder.Property(e => e.LastName)
            .HasMaxLength(100);

        builder.Property(e => e.Position)
            .HasMaxLength(100);

        builder.Property(e => e.Phone)
            .HasMaxLength(20);

        builder.Property(e => e.ImageProfile)
            .HasColumnType("text"); // Use text type for Base64 images (no length limit)

        builder.Property(e => e.Roles)
            .HasMaxLength(500);

        builder.Property(e => e.Group)
            .HasMaxLength(100);

        // Configure relationship with Department
        builder.HasOne(e => e.Departments)
            .WithMany()
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull); // Set to null when department is deleted

        // Table name
        builder.ToTable("Employees");
    }
}
