using Microsoft.AspNetCore.Identity;

namespace ProjectManagement.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public bool IsRevoked { get; set; }
    public DateTime RevokeStart { get; set; }
    public DateTime RevokeEnd { get; set; }
    public bool RequirePasswordChange { get; set; }
    public DateTime? LastPasswordChangeDate { get; set; }
}
