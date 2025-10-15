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

    public GetApplicationUserProfileQueryHandler(
        UserManager<ApplicationUser> userManager, 
        IUser user)
    {
        _userManager = userManager;
        _user = user;
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

        return new CurrentUserDto
        {
            UserName = appUser.UserName,
            Id = Guid.Parse(appUser.Id),
            RoleNames = roles.ToList(),
            DisplayName = $"{appUser.UserName}"
        };
    }
}
