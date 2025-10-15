using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Application.Authentication.Commands.Register;
using ProjectManagement.Application.Authentication.Commands.Login;

namespace ProjectManagement.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly IJwtTokenService _jwtTokenService;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService,
        IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user?.UserName;
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = userName,
        };

        var result = await _userManager.CreateAsync(user, password);

        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }

    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }

    public async Task<ApplicationUser?> FindByEmailOrUsernameAsync(string emailOrUsername)
    {
        // Try to find by email first
        var user = await _userManager.FindByEmailAsync(emailOrUsername);
        
        // If not found by email, try to find by username
        if (user == null)
        {
            user = await _userManager.FindByNameAsync(emailOrUsername);
        }

        return user;
    }

    public async Task<(Result Result, string UserId)> RegisterUserAsync(RegisterDto registerDto)
    {
        // Check if email already exists
        var existingUserByEmail = await _userManager.FindByEmailAsync(registerDto.Email);
        if (existingUserByEmail != null)
        {
            return (Result.Failure(new[] { "อีเมลนี้ถูกใช้งานแล้ว" }), string.Empty);
        }

        // Check if username already exists
        var existingUserByUsername = await _userManager.FindByNameAsync(registerDto.Username);
        if (existingUserByUsername != null)
        {
            return (Result.Failure(new[] { "ชื่อผู้ใช้นี้ถูกใช้งานแล้ว" }), string.Empty);
        }

        // Create new user
        var user = new ApplicationUser
        {
            UserName = registerDto.Username,
            Email = registerDto.Email,
            RequirePasswordChange = false,
            LastPasswordChangeDate = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            return (result.ToApplicationResult(), string.Empty);
        }

        return (Result.Success(), user.Id);
    }

    public async Task<(Result Result, LoginResponseDto Data)> AuthenticateAsync(string emailOrUsername, string password)
    {
        // Find user by email or username
        var user = await FindByEmailOrUsernameAsync(emailOrUsername);
        
        if (user == null)
        {
            return (Result.Failure(new[] { "อีเมลหรือรหัสผ่านไม่ถูกต้อง" }), null!);
        }

        // Check if user is revoked
        if (user.IsRevoked && DateTime.UtcNow >= user.RevokeStart && DateTime.UtcNow <= user.RevokeEnd)
        {
            return (Result.Failure(new[] { "บัญชีผู้ใช้ถูกระงับการใช้งาน" }), null!);
        }

        // Verify password
        var passwordValid = await _userManager.CheckPasswordAsync(user, password);
        if (!passwordValid)
        {
            return (Result.Failure(new[] { "อีเมลหรือรหัสผ่านไม่ถูกต้อง" }), null!);
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

        return (Result.Success(), response);
    }
}
