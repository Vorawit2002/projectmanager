using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class IncreaseImageProfileLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Increase ImageProfile column length for ApplicationUser
            migrationBuilder.AlterColumn<string>(
                name: "ImageProfile",
                table: "AspNetUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            // Increase ImageProfile column length for Employee
            migrationBuilder.AlterColumn<string>(
                name: "ImageProfile",
                table: "Employees",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert ImageProfile column length for ApplicationUser
            migrationBuilder.AlterColumn<string>(
                name: "ImageProfile",
                table: "AspNetUsers",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            // Revert ImageProfile column length for Employee
            migrationBuilder.AlterColumn<string>(
                name: "ImageProfile",
                table: "Employees",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
