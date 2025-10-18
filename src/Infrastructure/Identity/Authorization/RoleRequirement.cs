using Microsoft.AspNetCore.Authorization;

namespace ProjectManagement.Infrastructure.Identity.Authorization;

public class RoleRequirement : IAuthorizationRequirement
{
    public string[] AllowedRoles { get; }

    public RoleRequirement(params string[] allowedRoles)
    {
        AllowedRoles = allowedRoles ?? Array.Empty<string>();
    }
}
