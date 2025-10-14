using System.Security.Claims;

using ProjectManagement.Application.Common.Interfaces;

namespace ProjectManagement.Web.Services;

public class CurrentUser : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub")
                     ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? UserName
    {
        get => _httpContextAccessor.HttpContext?.User?.FindFirstValue("name");
        set => throw new NotSupportedException("Setting UserName is not supported.");
    }
}
