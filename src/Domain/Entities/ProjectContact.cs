using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Entities;
public class ProjectContact : BaseAuditableEntity
{
    public Guid ProjectId { get; set; }
    public virtual Project? Projects { get; set; }
    public Guid OrganizationContactId { get; set; }
    public virtual OrganizationContact? OrganizationContacts { get; set; }

}
