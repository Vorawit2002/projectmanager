using Microsoft.AspNetCore.Identity;

namespace ProjectManagement.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    // Account security
    public bool IsRevoked { get; set; }
    public DateTime RevokeStart { get; set; }
    public DateTime RevokeEnd { get; set; }
    public bool RequirePasswordChange { get; set; }
    public DateTime? LastPasswordChangeDate { get; set; }
    
    // Basic profile (for users without Employee record)
    public string? ImageProfile { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    // Navigation property to Employee (1:1 relationship)
    public virtual Employee? Employee { get; set; }
}
