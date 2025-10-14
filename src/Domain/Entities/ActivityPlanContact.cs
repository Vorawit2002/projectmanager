using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Entities;
public class ActivityPlanContact : BaseAuditableEntity
{
    public Guid ActivityPlanId { get; set; }
    public virtual ActivityPlan? ActivityPlan { get; set; }
    public Guid OrganizationContactId { get; set; }
    public virtual OrganizationContact? OrganizationContacts { get; set; }

}
