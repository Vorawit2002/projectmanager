using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Authentication.Queries;
public class CurrentUserDto
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? TitleName { get; set; }
    public string? Position { get; set; }
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? ImageProfile { get; set; }
    public string? Group { get; set; }
    public string? RoleHR { get; set; }
    public string? EmployeeId { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
    
    // Legacy properties for backward compatibility
    public Guid? Id { get; set; }
    public string? DisplayName { get; set; }
    public IList<string> RoleNames { get; set; } = new List<string>();
}

