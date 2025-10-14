using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProjectManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigrationCRM15092002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    RevokeStart = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RevokeEnd = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสรายการ"),
                    NameFile = table.Column<string>(type: "text", nullable: false, comment: "ชื่อไฟล์"),
                    PathFile = table.Column<string>(type: "text", nullable: false, comment: "ที่อยู่ของไฟล์"),
                    FileSize = table.Column<long>(type: "bigint", nullable: false, comment: "ขนาดไฟล์"),
                    FileExtension = table.Column<string>(type: "text", nullable: false, comment: "นามสกุลไฟล์"),
                    BucketOriginalName = table.Column<string>(type: "text", nullable: false, comment: "ชื่อที่เก็บไฟล์"),
                    BucketOriginalPath = table.Column<string>(type: "text", nullable: false, comment: "ที่อยู่ที่เก็บไฟล์"),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    GCRecord = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสรายการ"),
                    Name = table.Column<string>(type: "text", nullable: false, comment: "ชื่อแผนก"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, comment: "การใช้งาน"),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    GCRecord = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสรายการ"),
                    Subject = table.Column<string>(type: "text", nullable: false),
                    SentTo = table.Column<string>(type: "text", nullable: false),
                    Massage = table.Column<string>(type: "text", nullable: false),
                    SendBy = table.Column<string>(type: "text", nullable: false),
                    SendType = table.Column<string>(type: "text", nullable: true),
                    RefEntityId = table.Column<string>(type: "text", nullable: true),
                    RefEntityClass = table.Column<string>(type: "text", nullable: true),
                    SendDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SendStatus = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "วันที่สร้าง"),
                    CreatedBy = table.Column<string>(type: "text", nullable: true, comment: "ชื่อผู้สร้าง"),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "วันที่ปรับปรุง"),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true, comment: "ชื่อผู้ปรับปรุง"),
                    GCRecord = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true, comment: "ชื่อผู้ลบข้อมูล")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailMessageSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสรายการ"),
                    SettingCode = table.Column<string>(type: "text", nullable: false),
                    SettingName = table.Column<string>(type: "text", nullable: false),
                    MailSubject = table.Column<string>(type: "text", nullable: false),
                    MailBody = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Remark = table.Column<string>(type: "text", nullable: true),
                    Sendtime = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "วันที่สร้าง"),
                    CreatedBy = table.Column<string>(type: "text", nullable: true, comment: "ชื่อผู้สร้าง"),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "วันที่ปรับปรุง"),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true, comment: "ชื่อผู้ปรับปรุง"),
                    GCRecord = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true, comment: "ชื่อผู้ลบข้อมูล")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailMessageSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสรายการ"),
                    Name = table.Column<string>(type: "text", nullable: false, comment: "ชื่อ"),
                    EventTypeCode = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    GCRecord = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสรายการ"),
                    Name = table.Column<string>(type: "text", nullable: false, comment: "ชื่อหน่วยงาน"),
                    ShortName = table.Column<string>(type: "text", nullable: true, comment: "ชื่อย่อหน่วยงาน"),
                    TypeOrganization = table.Column<int>(type: "integer", nullable: true, comment: "ประเภทหน่วยงาน"),
                    Address = table.Column<string>(type: "text", nullable: true, comment: "ที่อยู่"),
                    Coordinates = table.Column<string>(type: "text", nullable: true, comment: "พิกัด"),
                    WebSite = table.Column<string>(type: "text", nullable: true, comment: "เว็บไซต์"),
                    Phone = table.Column<string>(type: "text", nullable: true, comment: "เบอร์โทร"),
                    Fax = table.Column<string>(type: "text", nullable: true, comment: "แฟกซ์"),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    GCRecord = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PushSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Endpoint = table.Column<string>(type: "text", nullable: false),
                    P256dh = table.Column<string>(type: "text", nullable: false),
                    Auth = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    GCRecord = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushSubscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SMTPSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสรายการ"),
                    ConfigName = table.Column<string>(type: "text", nullable: false),
                    SMTPServer = table.Column<string>(type: "text", nullable: false),
                    SMTPPort = table.Column<string>(type: "text", nullable: false),
                    SMTPAuthentication = table.Column<int>(type: "integer", nullable: false),
                    SMTPUserName = table.Column<string>(type: "text", nullable: false),
                    SMTPPassword = table.Column<string>(type: "text", nullable: false),
                    SMTPEnableSSL = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Remark = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "วันที่สร้าง"),
                    CreatedBy = table.Column<string>(type: "text", nullable: true, comment: "ชื่อผู้สร้าง"),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "วันที่ปรับปรุง"),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true, comment: "ชื่อผู้ปรับปรุง"),
                    GCRecord = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true, comment: "ชื่อผู้ลบข้อมูล")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SMTPSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TodoLists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Colour_Code = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    GCRecord = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสรายการ"),
                    UserId = table.Column<string>(type: "text", nullable: false, comment: "รหัสไอดีจากOpenid"),
                    TitleName = table.Column<string>(type: "text", nullable: true, comment: "คำนำหน้าชื่อ"),
                    FirstName = table.Column<string>(type: "text", nullable: true, comment: "ชื่อ"),
                    LastName = table.Column<string>(type: "text", nullable: true, comment: "นามสกุล"),
                    Email = table.Column<string>(type: "text", nullable: false, comment: "อีเมล"),
                    Position = table.Column<string>(type: "text", nullable: true, comment: "ตำแหน่ง"),
                    Phone = table.Column<string>(type: "text", nullable: true, comment: "เบอร์โทรศัพท์"),
                    ImageProfile = table.Column<string>(type: "text", nullable: true),
                    isActive = table.Column<bool>(type: "boolean", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: true, comment: "รหัสไอดีของแผนก"),
                    Subscription = table.Column<bool>(type: "boolean", nullable: true),
                    Roles = table.Column<string>(type: "text", nullable: true),
                    Group = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    GCRecord = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmailScheduleSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสรายการ"),
                    ScheduleName = table.Column<string>(type: "text", maxLength: 2147483647, nullable: false, comment: "ชื่อการกำหนดเวลา"),
                    HangfireJobId = table.Column<string>(type: "text", nullable: true),
                    EmailMessageSettingId = table.Column<Guid>(type: "uuid", nullable: false),
                    SendMailDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "กำหนดวันที่ส่ง"),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false, comment: "สถานะเปิดใช้งาน"),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "วันที่สร้าง"),
                    CreatedBy = table.Column<string>(type: "text", nullable: true, comment: "ชื่อผู้สร้าง"),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "วันที่ปรับปรุง"),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true, comment: "ชื่อผู้ปรับปรุง"),
                    GCRecord = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true, comment: "ชื่อผู้ลบข้อมูล")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailScheduleSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailScheduleSettings_EmailMessageSettings_EmailMessageSett~",
                        column: x => x.EmailMessageSettingId,
                        principalTable: "EmailMessageSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสรายการ"),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสไอดีของหน่วยงาน"),
                    TitleName = table.Column<string>(type: "text", nullable: true, comment: "คำนำหน้าชื่อ"),
                    FirstName = table.Column<string>(type: "text", nullable: false, comment: "ชื่อ"),
                    LastName = table.Column<string>(type: "text", nullable: true, comment: "นามสกุล"),
                    Position = table.Column<string>(type: "text", nullable: true, comment: "ตำแหน่ง"),
                    Email = table.Column<string>(type: "text", nullable: true, comment: "อีเมล"),
                    Phone = table.Column<string>(type: "text", nullable: true, comment: "เบอร์โทร"),
                    Fax = table.Column<string>(type: "text", nullable: true, comment: "แฟกซ์"),
                    LineId = table.Column<string>(type: "text", nullable: true, comment: "ไลน์ไอดี"),
                    AttachmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    GCRecord = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationContacts_Attachments_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrganizationContacts_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสรายการ"),
                    ProjectCode = table.Column<string>(type: "text", nullable: false, comment: "รหัสโปรเจค"),
                    ProjectName = table.Column<string>(type: "text", nullable: false, comment: "ชื่อโปรเจค"),
                    ShortName = table.Column<string>(type: "text", nullable: true, comment: "ชื่อย่อโปรเจค"),
                    ContractNumber = table.Column<string>(type: "text", nullable: true, comment: "เลขที่สัญญา"),
                    ContractSignedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, comment: "วันที่ลงนามสัญญา"),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, comment: "วันที่เริ่มงาน"),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, comment: "วันที่สิ้นสุด"),
                    WarrantyEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, comment: "วันที่สิ้นสุดรับประกันผลงาน"),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: true, comment: "หน่วยงาน"),
                    ProjectCost = table.Column<decimal>(type: "numeric", nullable: true, comment: "งบประมาณโครงการ"),
                    ProjectType = table.Column<int>(type: "integer", nullable: true, comment: "ประเภทโปรเจค"),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    GCRecord = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TodoItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ListId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Reminder = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Done = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    GCRecord = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TodoItems_TodoLists_ListId",
                        column: x => x.ListId,
                        principalTable: "TodoLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสรายการ"),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสไอดีของพนักงาน"),
                    Objective = table.Column<string>(type: "text", nullable: true, comment: "วัตถุประสงค์"),
                    ObjectiveDetail = table.Column<string>(type: "text", nullable: true, comment: "รายละเอียดวัตถุประสงค์"),
                    detail = table.Column<string>(type: "text", nullable: true, comment: "รายละเอียด"),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true, comment: "รหัสไอดีโปรเจค"),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: true, comment: "รหัสไอดีหน่วยงาน"),
                    AllDay = table.Column<bool>(type: "boolean", nullable: true, comment: "ทั้งวันหรือไม่"),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "วันเวลาที่เริ่มต้น"),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "วันเวลาที่สิ้นสุด"),
                    Location = table.Column<string>(type: "text", nullable: true, comment: "สถานที่"),
                    HaveCost = table.Column<bool>(type: "boolean", nullable: true, comment: "มี/ไม่มีค่าใช้จ่าย"),
                    CostDetail = table.Column<string>(type: "text", nullable: true, comment: "รายละเอียดของค่าใช้จ่าย"),
                    Cost = table.Column<decimal>(type: "numeric", nullable: true, comment: "ค่าใช้จ่าย"),
                    OutSide = table.Column<bool>(type: "boolean", nullable: true),
                    EventTypeId = table.Column<Guid>(type: "uuid", nullable: true, comment: "รหัสไอดีประเภทกิจกรรม"),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    GCRecord = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityPlans_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityPlans_EventTypes_EventTypeId",
                        column: x => x.EventTypeId,
                        principalTable: "EventTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActivityPlans_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActivityPlans_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CheckInCheckOuts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสรายการ"),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสไอดีพนักงาน"),
                    Location = table.Column<string>(type: "text", nullable: true, comment: "สถานที่่"),
                    LocationCheckOut = table.Column<string>(type: "text", nullable: true, comment: "สถานที่ออกงาน"),
                    Lat = table.Column<string>(type: "text", nullable: true),
                    Long = table.Column<string>(type: "text", nullable: true),
                    IPAddress = table.Column<string>(type: "text", nullable: true, comment: "ip เครื่อง"),
                    CheckIn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "เข้างาน"),
                    CheckOut = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, comment: "ออกงาน"),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: true, comment: "รหัสไอดีโปรเจค"),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true, comment: "รหัสไอดีโปรเจค"),
                    CheckInCheckOutTypes = table.Column<int>(type: "integer", nullable: true),
                    Types = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    GCRecord = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckInCheckOuts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckInCheckOuts_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CheckInCheckOuts_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CheckInCheckOuts_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProjectContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสรายการ"),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสโปรเจค"),
                    OrganizationContactId = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสไอดีของผู้ติดต่อ"),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    GCRecord = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectContacts_OrganizationContacts_OrganizationContactId",
                        column: x => x.OrganizationContactId,
                        principalTable: "OrganizationContacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectContacts_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityPlanAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสรายการ"),
                    ActivityPlanId = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสไอดีของการนัดหมาย"),
                    AttachmentId = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสไอดีของไฟล์"),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    GCRecord = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityPlanAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityPlanAttachments_ActivityPlans_ActivityPlanId",
                        column: x => x.ActivityPlanId,
                        principalTable: "ActivityPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityPlanAttachments_Attachments_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityPlanContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสรายการ"),
                    ActivityPlanId = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสไอดีของการนัดหมาย"),
                    OrganizationContactId = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสไอดีหน่วยงาน"),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    GCRecord = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityPlanContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityPlanContacts_ActivityPlans_ActivityPlanId",
                        column: x => x.ActivityPlanId,
                        principalTable: "ActivityPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityPlanContacts_OrganizationContacts_OrganizationConta~",
                        column: x => x.OrganizationContactId,
                        principalTable: "OrganizationContacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสรายการ"),
                    ActivityPlanId = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสไอดีของการนัดหมาย"),
                    Summary = table.Column<string>(type: "text", nullable: false, comment: "สรุปรายงาน"),
                    ToDoNext = table.Column<string>(type: "text", nullable: true, comment: "สิ่งที่ต้องติดตามต่อ"),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    GCRecord = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanNotes_ActivityPlans_ActivityPlanId",
                        column: x => x.ActivityPlanId,
                        principalTable: "ActivityPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CheckInCheckOutAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสรายการ"),
                    CheckInCheckOutId = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสไอดีของการนัดหมาย"),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    AttachmentId = table.Column<Guid>(type: "uuid", nullable: false, comment: "รหัสไอดีของไฟล์"),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    GCRecord = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckInCheckOutAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckInCheckOutAttachments_Attachments_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CheckInCheckOutAttachments_CheckInCheckOuts_CheckInCheckOut~",
                        column: x => x.CheckInCheckOutId,
                        principalTable: "CheckInCheckOuts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityPlanAttachments_ActivityPlanId",
                table: "ActivityPlanAttachments",
                column: "ActivityPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityPlanAttachments_AttachmentId",
                table: "ActivityPlanAttachments",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityPlanContacts_ActivityPlanId",
                table: "ActivityPlanContacts",
                column: "ActivityPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityPlanContacts_OrganizationContactId",
                table: "ActivityPlanContacts",
                column: "OrganizationContactId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityPlans_EmployeeId",
                table: "ActivityPlans",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityPlans_EventTypeId",
                table: "ActivityPlans",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityPlans_OrganizationId",
                table: "ActivityPlans",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityPlans_ProjectId",
                table: "ActivityPlans",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CheckInCheckOutAttachments_AttachmentId",
                table: "CheckInCheckOutAttachments",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckInCheckOutAttachments_CheckInCheckOutId",
                table: "CheckInCheckOutAttachments",
                column: "CheckInCheckOutId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckInCheckOuts_EmployeeId",
                table: "CheckInCheckOuts",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckInCheckOuts_OrganizationId",
                table: "CheckInCheckOuts",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckInCheckOuts_ProjectId",
                table: "CheckInCheckOuts",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailScheduleSettings_EmailMessageSettingId",
                table: "EmailScheduleSettings",
                column: "EmailMessageSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationContacts_AttachmentId",
                table: "OrganizationContacts",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationContacts_OrganizationId",
                table: "OrganizationContacts",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanNotes_ActivityPlanId",
                table: "PlanNotes",
                column: "ActivityPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectContacts_OrganizationContactId",
                table: "ProjectContacts",
                column: "OrganizationContactId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectContacts_ProjectId",
                table: "ProjectContacts",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OrganizationId",
                table: "Projects",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_ListId",
                table: "TodoItems",
                column: "ListId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityPlanAttachments");

            migrationBuilder.DropTable(
                name: "ActivityPlanContacts");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CheckInCheckOutAttachments");

            migrationBuilder.DropTable(
                name: "EmailLogs");

            migrationBuilder.DropTable(
                name: "EmailScheduleSettings");

            migrationBuilder.DropTable(
                name: "PlanNotes");

            migrationBuilder.DropTable(
                name: "ProjectContacts");

            migrationBuilder.DropTable(
                name: "PushSubscriptions");

            migrationBuilder.DropTable(
                name: "SMTPSettings");

            migrationBuilder.DropTable(
                name: "TodoItems");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "CheckInCheckOuts");

            migrationBuilder.DropTable(
                name: "EmailMessageSettings");

            migrationBuilder.DropTable(
                name: "ActivityPlans");

            migrationBuilder.DropTable(
                name: "OrganizationContacts");

            migrationBuilder.DropTable(
                name: "TodoLists");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "EventTypes");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}
