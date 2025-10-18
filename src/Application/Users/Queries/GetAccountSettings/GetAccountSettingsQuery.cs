using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;

namespace ProjectManagement.Application.Users.Queries.GetAccountSettings;

public record GetAccountSettingsQuery : IRequest<Result<AccountSettingsDto>>
{
    public string UserId { get; set; } = default!;
}

public class GetAccountSettingsQueryHandler : IRequestHandler<GetAccountSettingsQuery, Result<AccountSettingsDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public GetAccountSettingsQueryHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<Result<AccountSettingsDto>> Handle(GetAccountSettingsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get user from Identity
            var user = await _identityService.GetUserByIdAsync(request.UserId);
            
            if (user == null)
            {
                return Result<AccountSettingsDto>.Failure(new[] { "User not found" });
            }

            // Get employee data if exists
            var employee = await _context.Employees
                .Include(e => e.Departments)
                .FirstOrDefaultAsync(e => e.UserId == user.Id, cancellationToken);

            // Get user roles
            var roles = await _identityService.GetUserRolesAsync(user.Id);

            var accountSettingsDto = new AccountSettingsDto
            {
                UserId = user.Id,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                
                // Fallback: Try Employee first, then ApplicationUser (from email)
                FirstName = employee?.FirstName ?? ExtractFirstNameFromEmail(user.Email),
                LastName = employee?.LastName ?? ExtractLastNameFromEmail(user.Email),
                
                // Employee-specific fields
                TitleName = employee?.TitleName,
                Position = employee?.Position,
                Phone = employee?.Phone,
                ImageProfile = employee?.ImageProfile,
                
                // Department information
                Department = employee?.Departments?.Name,
                DepartmentId = employee?.DepartmentId,
                
                // Additional user information
                Roles = roles.ToList(),
                IsActive = employee?.isActive ?? !user.IsRevoked
            };

            return Result<AccountSettingsDto>.Success(accountSettingsDto);
        }
        catch (Exception ex)
        {
            return Result<AccountSettingsDto>.Failure(new[] { $"Error retrieving account settings: {ex.Message}" });
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
