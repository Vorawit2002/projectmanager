using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Entities;
public class Department:BaseAuditableEntity
{
    public string Name { get; set; } = default!;
    public bool IsActive { get; set; } = default!;
}
