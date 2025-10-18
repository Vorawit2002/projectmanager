using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedRolesAndDefaultAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Generate GUIDs for roles
            var adminRoleId = Guid.NewGuid().ToString();
            var managerRoleId = Guid.NewGuid().ToString();
            var userRoleId = Guid.NewGuid().ToString();
            var viewerRoleId = Guid.NewGuid().ToString();

            // Seed Roles
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[,]
                {
                    { adminRoleId, "Admin", "ADMIN", Guid.NewGuid().ToString() },
                    { managerRoleId, "Manager", "MANAGER", Guid.NewGuid().ToString() },
                    { userRoleId, "User", "USER", Guid.NewGuid().ToString() },
                    { viewerRoleId, "Viewer", "VIEWER", Guid.NewGuid().ToString() }
                });

            // Generate GUID for default admin user
            var adminUserId = Guid.NewGuid().ToString();
            
            // Create default admin user
            // Password: Admin@123
            // This is the hashed version of "Admin@123" using ASP.NET Core Identity's default hasher
            var passwordHash = "AQAAAAEAACcQAAAAELT7H8LfmKEZpnUE1ojkOsZd2LzKrKic9n+U7r/mdmE6KeWngfdYRIvfcCfqF0ugrQ==";
            
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", 
                                "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed", 
                                "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount", "IsRevoked", 
                                "RevokeStart", "RevokeEnd", "RequirePasswordChange", "LastPasswordChangeDate" },
                values: new object[] 
                { 
                    adminUserId, 
                    "admin", 
                    "ADMIN", 
                    "admin@projectmanagement.com", 
                    "ADMIN@PROJECTMANAGEMENT.COM", 
                    true,
                    passwordHash,
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                    null,
                    false,
                    false,
                    null,
                    true,
                    0,
                    false,
                    DateTime.UtcNow,
                    DateTime.UtcNow,
                    false,
                    DateTime.UtcNow
                });

            // Assign Admin role to default admin user
            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { adminUserId, adminRoleId });

            // Create corresponding Employee record for admin user
            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "UserId", "TitleName", "FirstName", "LastName", "Email", "Position", 
                                "Phone", "ImageProfile", "isActive", "DepartmentId", "Subscription", "Roles", 
                                "Group", "Created", "CreatedBy", "LastModified", "LastModifiedBy", "GCRecord", 
                                "DeletedOnUtc", "DeletedBy" },
                values: new object[] 
                { 
                    Guid.NewGuid(), 
                    adminUserId, 
                    "Mr.", 
                    "Admin", 
                    "System", 
                    "admin@projectmanagement.com", 
                    "System Administrator",
                    null,
                    null,
                    true,
                    null,
                    false,
                    "Admin",
                    null,
                    DateTime.UtcNow,
                    "System",
                    DateTime.UtcNow,
                    "System",
                    false,
                    DateTime.UtcNow,
                    null
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove default admin user's employee record
            migrationBuilder.Sql("DELETE FROM \"Employees\" WHERE \"Email\" = 'admin@projectmanagement.com'");

            // Remove default admin user's role assignment
            migrationBuilder.Sql("DELETE FROM \"AspNetUserRoles\" WHERE \"UserId\" IN (SELECT \"Id\" FROM \"AspNetUsers\" WHERE \"UserName\" = 'admin')");

            // Remove default admin user
            migrationBuilder.Sql("DELETE FROM \"AspNetUsers\" WHERE \"UserName\" = 'admin'");

            // Remove seeded roles
            migrationBuilder.Sql("DELETE FROM \"AspNetRoles\" WHERE \"Name\" IN ('Admin', 'Manager', 'User', 'Viewer')");
        }
    }
}
