using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.ActivityPlans.Queries;
public class ActivityPlanNotificationDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public virtual Employee? Employees { get; set; }
    public string? Objective { get; set; } 
    public Guid? OrganizationId { get; set; }
    public virtual Organization? Organizations { get; set; }
    public bool? AllDay { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Title { get; set; } = default!;
}
