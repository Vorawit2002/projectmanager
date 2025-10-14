using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjectManagement.Application.Authentication.Queries;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Constants;
using ProjectManagement.Domain.Entities;
namespace ProjectManagement.Application.ApplicationUserProfile.Queries;

public record GetApplicationUserProfileCommand : IRequest<AuthenticationUserDto>;
public class GetApplicationUserProfileQueryHandler : IRequestHandler<GetApplicationUserProfileCommand, AuthenticationUserDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUser _user;
    private readonly RoleManager<IdentityRole> _roleManager;

    public GetApplicationUserProfileQueryHandler(IApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager, IUser user, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
        _user = user;
        _roleManager = roleManager;
    }
    public async Task<AuthenticationUserDto> Handle(GetApplicationUserProfileCommand request, CancellationToken cancellationToken)
    {

        ApplicationUser? appUser = await _userManager.FindByNameAsync(_user.UserName ?? "");
        //Guard.Against.NotFound(_user.UserName ?? "", appUser);
        if (appUser == null)
        {
            return new AuthenticationUserDto
            {
                UserName = _user.UserName

            };
        }
        else
        {
            var roles = await _userManager.GetRolesAsync(appUser);

            return new AuthenticationUserDto
            {
                UserName = appUser.UserName,
                Id = Guid.Parse(appUser.Id),
                RoleNames = roles.ToList(),
            };
        }
    }
}
