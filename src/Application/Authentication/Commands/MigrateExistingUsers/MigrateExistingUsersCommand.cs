using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ProjectManagement.Application.Authentication.Commands.MigrateExistingUsers;

public record MigrateExistingUsersCommand : IRequest<Result<MigrationResultDto>>
{
    public string DefaultPassword { get; init; } = "ChangeMe123!";
}

public class MigrateExistingUsersCommandValidator : AbstractValidator<MigrateExistingUsersCommand>
{
    public MigrateExistingUsersCommandValidator()
    {
        RuleFor(v => v.DefaultPassword)
            .NotEmpty().WithMessage("กรุณากำหนดรหัสผ่านเริ่มต้น")
            .MinimumLength(6).WithMessage("รหัสผ่านต้องมีความยาวอย่างน้อย 6 ตัวอักษร");
    }
}

public class MigrateExistingUsersCommandHandler : IRequestHandler<MigrateExistingUsersCommand, Result<MigrationResultDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _context;

    public MigrateExistingUsersCommandHandler(
        UserManager<ApplicationUser> userManager,
        IApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<Result<MigrationResultDto>> Handle(MigrateExistingUsersCommand request, CancellationToken cancellationToken)
    {
        var result = new MigrationResultDto
        {
            TotalUsers = 0,
            MigratedUsers = 0,
            SkippedUsers = 0,
            FailedUsers = 0,
            Errors = new List<string>()
        };

        try
        {
            // Get all users from the database
            var allUsers = _userManager.Users.ToList();
            result.TotalUsers = allUsers.Count;

            foreach (var user in allUsers)
            {
                try
                {
                    // Check if user already has a password
                    var hasPassword = await _userManager.HasPasswordAsync(user);
                    
                    if (hasPassword)
                    {
                        // User already has a password, skip
                        result.SkippedUsers++;
                        continue;
                    }

                    // Set default password for users without password
                    var addPasswordResult = await _userManager.AddPasswordAsync(user, request.DefaultPassword);
                    
                    if (!addPasswordResult.Succeeded)
                    {
                        result.FailedUsers++;
                        result.Errors.Add($"Failed to set password for user {user.UserName}: {string.Join(", ", addPasswordResult.Errors.Select(e => e.Description))}");
                        continue;
                    }

                    // Set RequirePasswordChange flag
                    user.RequirePasswordChange = true;
                    user.LastPasswordChangeDate = null;

                    var updateResult = await _userManager.UpdateAsync(user);
                    
                    if (!updateResult.Succeeded)
                    {
                        result.FailedUsers++;
                        result.Errors.Add($"Failed to update user {user.UserName}: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");
                        continue;
                    }

                    // Get and preserve existing roles
                    var roles = await _userManager.GetRolesAsync(user);
                    
                    result.MigratedUsers++;
                    result.UserDetails.Add(new MigratedUserDto
                    {
                        UserId = user.Id,
                        Username = user.UserName ?? string.Empty,
                        Email = user.Email ?? string.Empty,
                        Roles = roles.ToList()
                    });
                }
                catch (Exception ex)
                {
                    result.FailedUsers++;
                    result.Errors.Add($"Exception for user {user.UserName}: {ex.Message}");
                }
            }

            return Result<MigrationResultDto>.Success(result);
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Migration failed: {ex.Message}");
            return Result<MigrationResultDto>.Failure(result.Errors.ToArray());
        }
    }
}

public class MigrationResultDto
{
    public int TotalUsers { get; set; }
    public int MigratedUsers { get; set; }
    public int SkippedUsers { get; set; }
    public int FailedUsers { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<MigratedUserDto> UserDetails { get; set; } = new();
}

public class MigratedUserDto
{
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}
