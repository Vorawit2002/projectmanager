using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Entities;
public class EventType : BaseAuditableEntity
{
    public string Name { get; set; } = default!;
    public string? EventTypeCode { get; set; }
}
