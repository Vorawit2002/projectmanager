using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Constants;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Identity.Authorization;

public class DepartmentAuthorizationHandler : AuthorizationHandler<DepartmentRequirement, Employee>
{
    private readonly IApplicationDbContext _context;

    public DepartmentAuthorizationHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        DepartmentRequirement requirement,
        Employee resource)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        // Check if user is Admin - full access
        if (context.User.IsInRole(Roles.Admin) || context.User.IsInRole(Roles.Administrator))
        {
            context.Succeed(requirement);
            return;
        }

        // Get current user's employee record
        var currentEmployee = await _context.Employees
            .FirstOrDefaultAsync(e => e.UserId == userId);

        if (currentEmployee == null)
        {
            return;
        }

        // Check if user is Manager in the same department
        if (context.User.IsInRole(Roles.Manager))
        {
            if (requirement.RequireSameDepartment)
            {
                if (currentEmployee.DepartmentId.HasValue && 
                    resource.DepartmentId.HasValue &&
                    currentEmployee.DepartmentId == resource.DepartmentId)
                {
                    context.Succeed(requirement);
                    return;
                }
            }
            else
            {
                // Manager can access without department check
                context.Succeed(requirement);
                return;
            }
        }

        // Check if user is accessing their own data
        if (context.User.IsInRole(Roles.User) || context.User.IsInRole(Roles.Viewer))
        {
            if (resource.UserId == userId)
            {
                context.Succeed(requirement);
                return;
            }
        }
    }
}
