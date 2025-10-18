using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Users.Queries.GetAllUsers;

namespace ProjectManagement.Application.Users.Queries.GetUserById;

public record GetUserByIdQuery : IRequest<Result<UserDto>>
{
    public string UserId { get; set; } = default!;
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public GetUserByIdQueryHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get user from Identity
            var user = await _identityService.GetUserByIdAsync(request.UserId);
            
            if (user == null)
            {
                return Result<UserDto>.Failure(new[] { "User not found" });
            }

            // Get employee data if exists
            var employee = await _context.Employees
                .Include(e => e.Departments)
                .FirstOrDefaultAsync(e => e.UserId == user.Id, cancellationToken);

            // Get user roles
            var roles = await _identityService.GetUserRolesAsync(user.Id);

            var userDto = new UserDto
            {
                UserId = user.Id,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FirstName = employee?.FirstName,
                LastName = employee?.LastName,
                ImageProfile = employee?.ImageProfile,
                Department = employee?.Departments?.Name,
                DepartmentId = employee?.DepartmentId,
                Roles = roles.ToList(),
                IsActive = employee?.isActive ?? !user.IsRevoked,
                LastLoginDate = user.LastPasswordChangeDate
            };

            return Result<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            return Result<UserDto>.Failure(new[] { $"Error retrieving user: {ex.Message}" });
        }
    }
}
