using Microsoft.AspNetCore.Authorization;

namespace ProjectManagement.Infrastructure.Identity.Authorization;

public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RoleRequirement requirement)
    {
        if (requirement.AllowedRoles == null || requirement.AllowedRoles.Length == 0)
        {
            return Task.CompletedTask;
        }

        // Check if user has any of the allowed roles
        foreach (var role in requirement.AllowedRoles)
        {
            if (context.User.IsInRole(role))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }

        return Task.CompletedTask;
    }
}
