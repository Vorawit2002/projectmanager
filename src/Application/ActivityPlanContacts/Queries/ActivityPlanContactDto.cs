using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.ActivityPlanContacts.Queries;
public class ActivityPlanContactDto
{
    public Guid Id { get; set; }
    public Guid ActivityPlanId { get; set; }
    public virtual ActivityPlan? ActivityPlan { get; set; }
    public Guid OrganizationContactId { get; set; }
    public virtual OrganizationContact? OrganizationContacts { get; set; }
    public DateTime Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime LastModified { get; set; }
    public string? LastModifiedBy { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ActivityPlanContact, ActivityPlanContactDto>();
        }
    }
}
