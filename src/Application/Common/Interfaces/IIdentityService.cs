using ProjectManagement.Application.Authentication.Commands.Login;
using ProjectManagement.Application.Authentication.Commands.Register;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);

    Task<Result> DeleteUserAsync(string userId);

    // New methods for local authentication
    Task<ApplicationUser?> FindByEmailOrUsernameAsync(string emailOrUsername);
    
    Task<(Result Result, string UserId)> RegisterUserAsync(RegisterDto registerDto);
    
    Task<(Result Result, LoginResponseDto Data)> AuthenticateAsync(string emailOrUsername, string password);

    // User management methods
    Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
    
    Task<ApplicationUser?> GetUserByIdAsync(string userId);
    
    Task<Result> AssignRoleAsync(string userId, string roleName);
    
    Task<Result> RemoveRoleAsync(string userId, string roleName);
    
    Task<IEnumerable<string>> GetUserRolesAsync(string userId);
    
    Task<IEnumerable<ApplicationUser>> GetUsersByDepartmentAsync(Guid departmentId);
    
    Task<Result> UpdateUserAsync(ApplicationUser user);
}
