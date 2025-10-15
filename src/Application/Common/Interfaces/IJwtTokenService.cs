using System.Security.Claims;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
    ClaimsPrincipal? ValidateToken(string token);
    DateTime GetTokenExpiration(string token);
}
