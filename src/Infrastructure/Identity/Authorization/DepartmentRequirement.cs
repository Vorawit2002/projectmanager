using Microsoft.AspNetCore.Authorization;

namespace ProjectManagement.Infrastructure.Identity.Authorization;

public class DepartmentRequirement : IAuthorizationRequirement
{
    public bool RequireSameDepartment { get; }

    public DepartmentRequirement(bool requireSameDepartment = true)
    {
        RequireSameDepartment = requireSameDepartment;
    }
}
