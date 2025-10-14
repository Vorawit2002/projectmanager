using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Entities;
public class PlanNote:BaseAuditableEntity
{
    public Guid ActivityPlanId { get; set; }
    public virtual ActivityPlan? ActivityPlans { get; set; }
    public string Summary { get; set; } = default!;
    public string? ToDoNext { get; set; }
    public string? Remarks { get; set; }
}
