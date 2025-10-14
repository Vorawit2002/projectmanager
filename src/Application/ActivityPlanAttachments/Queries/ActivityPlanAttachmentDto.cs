using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.ActivityPlanAttachments.Queries;
public class ActivityPlanAttachmentDto
{
    public Guid Id { get; set; }
    public Guid ActivityPlanId { get; set; } = default!;
    public virtual ActivityPlan ActivityPlans { get; set; } = default!;
    public Guid AttachmentId { get; set; } = default!;
    public virtual Attachment Attachments { get; set; } = default!;
    public string ThumbnailBase64 { get; set; } = default!;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ActivityPlanAttachment, ActivityPlanAttachmentDto>();
        }
    }
}
