using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;

namespace ProjectManagement.Application.Users.Queries.GetAllUsers;

public record GetAllUsersQuery : IRequest<Result<List<UserDto>>>
{
    public string? SearchTerm { get; set; }
    public string? RoleFilter { get; set; }
    public bool? IsActiveFilter { get; set; }
}

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<List<UserDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public GetAllUsersQueryHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<Result<List<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get all users from Identity
            var users = await _identityService.GetAllUsersAsync();
            
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
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
                    
                    // Priority: Employee > ApplicationUser > Extract from email
                    FirstName = employee?.FirstName ?? user.FirstName ?? ExtractFirstNameFromEmail(user.Email),
                    LastName = employee?.LastName ?? user.LastName ?? ExtractLastNameFromEmail(user.Email),
                    
                    // Priority: ApplicationUser.ImageProfile > Employee.ImageProfile
                    ImageProfile = user.ImageProfile ?? employee?.ImageProfile,
                    
                    Position = employee?.Position,
                    Department = employee?.Departments?.Name,
                    DepartmentId = employee?.DepartmentId,
                    Roles = roles.ToList(),
                    IsActive = employee?.isActive ?? !user.IsRevoked,
                    LastLoginDate = user.LastPasswordChangeDate
                };

                userDtos.Add(userDto);
            }

            // Apply filters
            var filteredUsers = userDtos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                filteredUsers = filteredUsers.Where(u =>
                    (u.Username != null && u.Username.ToLower().Contains(searchTerm)) ||
                    (u.Email != null && u.Email.ToLower().Contains(searchTerm)) ||
                    (u.FirstName != null && u.FirstName.ToLower().Contains(searchTerm)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(searchTerm)) ||
                    (u.Department != null && u.Department.ToLower().Contains(searchTerm))
                );
            }

            if (!string.IsNullOrWhiteSpace(request.RoleFilter))
            {
                filteredUsers = filteredUsers.Where(u => u.Roles.Contains(request.RoleFilter));
            }

            if (request.IsActiveFilter.HasValue)
            {
                filteredUsers = filteredUsers.Where(u => u.IsActive == request.IsActiveFilter.Value);
            }

            return Result<List<UserDto>>.Success(filteredUsers.ToList());
        }
        catch (Exception ex)
        {
            return Result<List<UserDto>>.Failure(new[] { $"Error retrieving users: {ex.Message}" });
        }
    }

    private string? ExtractFirstNameFromEmail(string? email)
    {
        if (string.IsNullOrEmpty(email))
            return null;

        var localPart = email.Split('@')[0];
        var parts = localPart.Split('.');
        
        return parts.Length > 0 ? CapitalizeFirstLetter(parts[0]) : null;
    }

    private string? ExtractLastNameFromEmail(string? email)
    {
        if (string.IsNullOrEmpty(email))
            return null;

        var localPart = email.Split('@')[0];
        var parts = localPart.Split('.');
        
        return parts.Length > 1 ? CapitalizeFirstLetter(parts[1]) : null;
    }

    private string CapitalizeFirstLetter(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        return char.ToUpper(text[0]) + text.Substring(1).ToLower();
    }
}
