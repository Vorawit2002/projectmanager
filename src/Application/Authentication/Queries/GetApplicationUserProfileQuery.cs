using ProjectManagement.Application.Authentication.Queries;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ProjectManagement.Application.ApplicationUserProfile.Queries;

public record GetApplicationUserProfileCommand : IRequest<CurrentUserDto>;

public class GetApplicationUserProfileQueryHandler : IRequestHandler<GetApplicationUserProfileCommand, CurrentUserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUser _user;
    private readonly IApplicationDbContext _context;

    public GetApplicationUserProfileQueryHandler(
        UserManager<ApplicationUser> userManager, 
        IUser user,
        IApplicationDbContext context)
    {
        _userManager = userManager;
        _user = user;
        _context = context;
    }

    public async Task<CurrentUserDto> Handle(GetApplicationUserProfileCommand request, CancellationToken cancellationToken)
    {
        // Get current user from JWT token claims
        var userId = _user.Id;
        
        if (string.IsNullOrEmpty(userId))
        {
            return new CurrentUserDto
            {
                UserName = _user.UserName
            };
        }

        // Find user by ID from JWT claims
        var appUser = await _userManager.FindByIdAsync(userId);
        
        if (appUser == null)
        {
            return new CurrentUserDto
            {
                UserName = _user.UserName
            };
        }

        // Get user roles
        var roles = await _userManager.GetRolesAsync(appUser);

        // Get employee data if exists
        var employee = await _context.Employees
            .Include(e => e.Departments)
            .FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken);

        return new CurrentUserDto
        {
            UserId = appUser.Id,
            UserName = appUser.UserName,
            Email = appUser.Email,
            FirstName = employee?.FirstName,
            LastName = employee?.LastName,
            TitleName = employee?.TitleName,
            Position = employee?.Position,
            Phone = employee?.Phone,
            Department = employee?.Departments?.Name,
            DepartmentId = employee?.DepartmentId,
            ImageProfile = appUser.ImageProfile ?? employee?.ImageProfile,
            Group = employee?.Group,
            RoleHR = employee?.Roles, // Use Roles property from Employee
            EmployeeId = employee?.Id.ToString(),
            Roles = roles.ToList(),
            
            // Legacy properties
            Id = Guid.Parse(appUser.Id),
            RoleNames = roles.ToList(),
            DisplayName = $"{employee?.FirstName} {employee?.LastName}".Trim()
        };
    }
}
