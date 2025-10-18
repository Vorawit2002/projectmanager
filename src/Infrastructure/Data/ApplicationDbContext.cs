using System.Reflection;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<ActivityPlan> ActivityPlans => Set<ActivityPlan>();

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<OrganizationContact> OrganizationContacts => Set<OrganizationContact>();

    public DbSet<ActivityPlanContact> ActivityPlanContacts => Set<ActivityPlanContact>();
    public DbSet<PlanNote> PlanNotes => Set<PlanNote>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectContact> ProjectContacts => Set<ProjectContact>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<ActivityPlanAttachment> ActivityPlanAttachments => Set<ActivityPlanAttachment>();
    public DbSet<EventType> EventTypes => Set<EventType>();
    public DbSet<EmailMessageSetting> EmailMessageSettings => Set<EmailMessageSetting>();
    public DbSet<SMTPSetting> SMTPSettings => Set<SMTPSetting>();
    public DbSet<EmailScheduleSetting> EmailScheduleSettings => Set<EmailScheduleSetting>();
    public DbSet<EmailLog> EmailLogs => Set<EmailLog>();
    public DbSet<CheckInCheckOut> CheckInCheckOuts => Set<CheckInCheckOut>();
    public DbSet<CheckInCheckOutAttachment> CheckInCheckOutAttachments => Set<CheckInCheckOutAttachment>();
    public DbSet<PushSubscription> PushSubscriptions => Set<PushSubscription>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Entity<Department>().HasQueryFilter(r => !r.GCRecord);
        builder.Entity<ActivityPlan>().HasQueryFilter(r => !r.GCRecord);
        builder.Entity<Organization>().HasQueryFilter(r => !r.GCRecord);
        builder.Entity<ActivityPlanContact>().HasQueryFilter(r => !r.GCRecord);
        builder.Entity<Employee>().HasQueryFilter(r => !r.GCRecord);
        builder.Entity<OrganizationContact>().HasQueryFilter(r => !r.GCRecord);
        builder.Entity<PlanNote>().HasQueryFilter(r => !r.GCRecord);
        builder.Entity<Project>().HasQueryFilter(r => !r.GCRecord);
        builder.Entity<ProjectContact>().HasQueryFilter(r => !r.GCRecord);
        builder.Entity<Attachment>().HasQueryFilter(r => !r.GCRecord);
        builder.Entity<ActivityPlanAttachment>().HasQueryFilter(r => !r.GCRecord);
        builder.Entity<EventType>().HasQueryFilter(r => !r.GCRecord);
        builder.Entity<EmailMessageSetting>().HasQueryFilter(r => !r.GCRecord);
        builder.Entity<SMTPSetting>().HasQueryFilter(r => !r.GCRecord);
        builder.Entity<EmailScheduleSetting>().HasQueryFilter(r => !r.GCRecord);
        builder.Entity<EmailLog>().HasQueryFilter(r => !r.GCRecord);
        builder.Entity<CheckInCheckOut>().HasQueryFilter(r => !r.GCRecord);
        builder.Entity<CheckInCheckOutAttachment>().HasQueryFilter(r => !r.GCRecord);
    }
}
