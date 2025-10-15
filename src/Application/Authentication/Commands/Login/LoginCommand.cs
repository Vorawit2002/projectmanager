using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ProjectManagement.Application.Authentication.Commands.Login;

public record LoginCommand : IRequest<Result<LoginResponseDto>>
{
    public string EmailOrUsername { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(v => v.EmailOrUsername)
            .NotEmpty().WithMessage("กรุณากรอกอีเมลหรือชื่อผู้ใช้");

        RuleFor(v => v.Password)
            .NotEmpty().WithMessage("กรุณากรอกรหัสผ่าน");
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly UserManager<ApplicationUser> _userManager;

    public LoginCommandHandler(
        IIdentityService identityService,
        IJwtTokenService jwtTokenService,
        UserManager<ApplicationUser> userManager)
    {
        _identityService = identityService;
        _jwtTokenService = jwtTokenService;
        _userManager = userManager;
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Find user by email or username
        var user = await _identityService.FindByEmailOrUsernameAsync(request.EmailOrUsername);
        
        if (user == null)
        {
            return Result<LoginResponseDto>.Failure(new[] { "อีเมลหรือรหัสผ่านไม่ถูกต้อง" });
        }

        // Check if user is revoked
        if (user.IsRevoked && DateTime.UtcNow >= user.RevokeStart && DateTime.UtcNow <= user.RevokeEnd)
        {
            return Result<LoginResponseDto>.Failure(new[] { "บัญชีผู้ใช้ถูกระงับการใช้งาน" });
        }

        // Verify password
        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            return Result<LoginResponseDto>.Failure(new[] { "อีเมลหรือรหัสผ่านไม่ถูกต้อง" });
        }

        // Get user roles
        var roles = await _userManager.GetRolesAsync(user);

        // Generate JWT token
        var token = _jwtTokenService.GenerateToken(user, roles);
        var tokenExpiration = _jwtTokenService.GetTokenExpiration(token);

        var response = new LoginResponseDto
        {
            Token = token,
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
