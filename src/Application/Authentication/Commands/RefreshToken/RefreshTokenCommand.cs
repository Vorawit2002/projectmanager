using ProjectManagement.Application.Authentication.Commands.Login;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ProjectManagement.Application.Authentication.Commands.RefreshToken;

public record RefreshTokenCommand : IRequest<Result<LoginResponseDto>>
{
    public string Token { get; init; } = string.Empty;
}

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(v => v.Token)
            .NotEmpty().WithMessage("กรุณาระบุ token");
    }
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<LoginResponseDto>>
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly UserManager<ApplicationUser> _userManager;

    public RefreshTokenCommandHandler(
        IJwtTokenService jwtTokenService,
        UserManager<ApplicationUser> userManager)
    {
        _jwtTokenService = jwtTokenService;
        _userManager = userManager;
    }

    public async Task<Result<LoginResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Validate the token (even if expired, we want to extract the user info)
        var principal = _jwtTokenService.ValidateToken(request.Token);
        
        if (principal == null)
        {
            return Result<LoginResponseDto>.Failure(new[] { "Token ไม่ถูกต้อง" });
        }

        // Extract user ID from token
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Result<LoginResponseDto>.Failure(new[] { "ไม่พบข้อมูลผู้ใช้ใน token" });
        }

        // Get user from database
        var user = await _userManager.FindByIdAsync(userIdClaim.Value);
        if (user == null)
        {
            return Result<LoginResponseDto>.Failure(new[] { "ไม่พบผู้ใช้ในระบบ" });
        }

        // Check if user is revoked
        if (user.IsRevoked && DateTime.UtcNow >= user.RevokeStart && DateTime.UtcNow <= user.RevokeEnd)
        {
            return Result<LoginResponseDto>.Failure(new[] { "บัญชีผู้ใช้ถูกระงับการใช้งาน" });
        }

        // Get user roles
        var roles = await _userManager.GetRolesAsync(user);

        // Generate new JWT token
        var newToken = _jwtTokenService.GenerateToken(user, roles);
        var tokenExpiration = _jwtTokenService.GetTokenExpiration(newToken);

        var response = new LoginResponseDto
        {
            Token = newToken,
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            Username = user.UserName ?? string.Empty,
            Roles = roles.ToList(),
            TokenExpiration = tokenExpiration,
            RequirePasswordChange = user.RequirePasswordChange
        };

        return Result<LoginResponseDto>.Success(response);
    }
}
