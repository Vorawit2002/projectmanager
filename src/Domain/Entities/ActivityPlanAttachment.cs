using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace ProjectManagement.Domain.Entities;
public class ActivityPlanAttachment : BaseAuditableEntity
{
    public Guid ActivityPlanId { get; set; } = default!;
    public virtual ActivityPlan ActivityPlans { get; set; } = default!;
    public Guid AttachmentId { get; set; } = default!;
    public virtual Attachment Attachments { get; set; } = default!;
}
