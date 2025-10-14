using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.ActivityPlans.Queries;
public class ActivityPlanDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public virtual Employee? Employees { get; set; }
    public string? Objective { get; set; }
    public string? ObjectiveDetail { get; set; }
    public string? detail { get; set; } 
    public Guid? ProjectId { get; set; }
    public virtual Project? Projects { get; set; }
    public Guid? OrganizationId { get; set; }
    public virtual Organization? Organizations { get; set; }
    //public ActivityPlanStatus? ActivityPlanStatus { get; set; }
    public bool? AllDay { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Location { get; set; }
    public bool? HaveCost { get; set; }
    public string? CostDetail { get; set; }
    public decimal? Cost { get; set; }
    public bool? OutSide { get; set; }
    public Guid? EventTypeId { get; set; }
    public virtual EventType? EventTypes { get; set; } 
    public DateTime Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime LastModified { get; set; }
    public string? LastModifiedBy { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ActivityPlan, ActivityPlanDto>();
        }
    }
}
