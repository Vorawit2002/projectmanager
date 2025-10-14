using ProjectManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }
    DbSet<Department> Departments { get; }
    DbSet<ActivityPlan> ActivityPlans { get; }
    DbSet<Organization> Organizations { get; }
    DbSet<ActivityPlanContact> ActivityPlanContacts { get; }
    DbSet<Employee> Employees { get; }
    DbSet<OrganizationContact> OrganizationContacts { get; }
    DbSet<PlanNote> PlanNotes { get; }
    DbSet<Project> Projects { get; }
    DbSet<ProjectContact> ProjectContacts { get; }
    DbSet<Attachment> Attachments { get; }
    DbSet<ActivityPlanAttachment> ActivityPlanAttachments { get; }
    DbSet<EventType> EventTypes { get; }
    DbSet<EmailMessageSetting> EmailMessageSettings { get; }
    DbSet<SMTPSetting> SMTPSettings { get; }
    DbSet<EmailScheduleSetting> EmailScheduleSettings { get; }
    DbSet<EmailLog> EmailLogs { get; }
    DbSet<CheckInCheckOut> CheckInCheckOuts { get; }
    DbSet<CheckInCheckOutAttachment> CheckInCheckOutAttachments { get; }
    DbSet<PushSubscription> PushSubscriptions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
