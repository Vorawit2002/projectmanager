using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Entities;
public class OrganizationContact:BaseAuditableEntity
{
    public Guid OrganizationId { get; set; }
    public virtual Organization? Organizations { get; set; }
    public string TitleName { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string? LastName { get; set; }
    public string? Position { get; set; }
    public string? Email { get; set; } 
    public string? Phone { get; set; } 
    public string? Fax { get; set; }
    public string? LineId { get; set; }
    public Guid? AttachmentId { get; set; } 
    public virtual Attachment? Attachments { get; set; } 
}
