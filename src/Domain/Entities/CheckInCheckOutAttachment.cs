using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace ProjectManagement.Domain.Entities;
public class CheckInCheckOutAttachment : BaseAuditableEntity
{
    public Guid CheckInCheckOutId { get; set; } = default!;
    public virtual CheckInCheckOut CheckInCheckOuts { get; set; } = default!;
    public CheckInCheckOutType Type { get; set; } = default!; 
    public Guid AttachmentId { get; set; } = default!;
    public virtual Attachment Attachments { get; set; } = default!;
}
