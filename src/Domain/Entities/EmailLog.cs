using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Entities;
public class EmailLog : BaseAuditableEntity
{
    public string Subject { get; set; } = default!;
    public string SentTo { get; set; } = default!;
    public string Massage { get; set; } = default!;
    public string SendBy { get; set; } = default!;
    public string? SendType { get; set; }
    public string? RefEntityId { get; set; }
    public string? RefEntityClass { get; set; }
    public DateTime SendDate { get; set; }
    public bool SendStatus { get; set; }
}
