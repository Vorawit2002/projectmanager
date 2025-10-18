using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Entities;
public class Employee :BaseAuditableEntity
{
    // Foreign key to AspNetUsers
    public string UserId { get; set; } = default!;
    
    // Navigation property to ApplicationUser
    public virtual ApplicationUser? User { get; set; }
    
    // Employee profile information
    public string? TitleName { get; set; }
    public string? FirstName { get; set; } 
    public string? LastName { get; set; }
    public string Email { get; set; } = default!;
    public string? Position { get; set; }
    public string? Phone { get; set; }
    public string? ImageProfile { get; set; }
    public bool? isActive { get; set; }
    
    // Department relationship
    public Guid? DepartmentId { get; set; }
    public virtual Department? Departments { get; set; }
    
    // Additional fields
    public bool? Subscription { get; set; }
    public string? Roles { get; set; }  // Consider removing - use AspNetUserRoles instead
    public string? Group { get; set; }
}
