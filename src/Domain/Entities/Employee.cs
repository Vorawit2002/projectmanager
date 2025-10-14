using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Entities;
public class Employee :BaseAuditableEntity
{
    public string UserId { get; set; } = default!;
    public string? TitleName { get; set; }
    public string? FirstName { get; set; } 
    public string? LastName { get; set; }
    public string Email { get; set; } = default!;
    public string? Position { get; set; }
    public string? Phone { get; set; }
    public string? ImageProfile { get; set; }
    public bool? isActive { get; set; }
    public Guid? DepartmentId { get; set; }
    public virtual Department? Departments { get; set; }
    public bool? Subscription { get; set; }
    public string? Roles { get; set; }
    public string? Group { get; set; }
}
