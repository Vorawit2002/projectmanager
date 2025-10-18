using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Clean up ImageProfile data that exceeds max length
            migrationBuilder.Sql(@"
                UPDATE ""Employees"" 
                SET ""ImageProfile"" = NULL 
                WHERE LENGTH(""ImageProfile"") > 1000;
            ");

            // Delete Employee records that reference non-existent users
            migrationBuilder.Sql(@"
                DELETE FROM ""Employees"" 
                WHERE ""UserId"" NOT IN (SELECT ""Id"" FROM ""AspNetUsers"");
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Departments_DepartmentId",
                table: "Employees");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Employees",
                type: "character varying(450)",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldComment: "รหัสไอดีจากOpenid");

            migrationBuilder.AlterColumn<string>(
                name: "TitleName",
                table: "Employees",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldComment: "คำนำหน้าชื่อ");

            migrationBuilder.AlterColumn<string>(
                name: "Roles",
                table: "Employees",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Position",
                table: "Employees",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldComment: "ตำแหน่ง");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Employees",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldComment: "เบอร์โทรศัพท์");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Employees",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldComment: "นามสกุล");

            migrationBuilder.AlterColumn<string>(
                name: "ImageProfile",
                table: "Employees",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Group",
                table: "Employees",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Employees",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldComment: "ชื่อ");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Employees",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldComment: "อีเมล");

            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "Employees",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true,
                oldComment: "รหัสไอดีของแผนก");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Employees",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldComment: "รหัสรายการ");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UserId",
                table: "Employees",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_AspNetUsers_UserId",
                table: "Employees",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Departments_DepartmentId",
                table: "Employees",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_AspNetUsers_UserId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Departments_DepartmentId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_UserId",
                table: "Employees");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Employees",
                type: "text",
                nullable: false,
                comment: "รหัสไอดีจากOpenid",
                oldClrType: typeof(string),
                oldType: "character varying(450)",
                oldMaxLength: 450);

            migrationBuilder.AlterColumn<string>(
                name: "TitleName",
                table: "Employees",
                type: "text",
                nullable: true,
                comment: "คำนำหน้าชื่อ",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Roles",
                table: "Employees",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Position",
                table: "Employees",
                type: "text",
                nullable: true,
                comment: "ตำแหน่ง",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Employees",
                type: "text",
                nullable: true,
                comment: "เบอร์โทรศัพท์",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Employees",
                type: "text",
                nullable: true,
                comment: "นามสกุล",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageProfile",
                table: "Employees",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Group",
                table: "Employees",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Employees",
                type: "text",
                nullable: true,
                comment: "ชื่อ",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Employees",
                type: "text",
                nullable: false,
                comment: "อีเมล",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "Employees",
                type: "uuid",
                nullable: true,
                comment: "รหัสไอดีของแผนก",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Employees",
                type: "uuid",
                nullable: false,
                comment: "รหัสรายการ",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Departments_DepartmentId",
                table: "Employees",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");
        }
    }
}
