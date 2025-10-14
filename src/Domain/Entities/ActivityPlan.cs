using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Entities;
public class ActivityPlan :BaseAuditableEntity
{
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
    public IList<ActivityPlanContact> ActivityPlanContacts { get; set; } = new List<ActivityPlanContact>();
    public IList<ActivityPlanAttachment> ActivityPlanAttachments { get; set; } = new List<ActivityPlanAttachment>();
    public IList<PlanNote> PlanNotes { get; set; } = new List<PlanNote>();
}
