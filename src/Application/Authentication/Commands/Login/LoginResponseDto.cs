namespace ProjectManagement.Application.Authentication.Commands.Login;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public DateTime TokenExpiration { get; set; }
    public bool RequirePasswordChange { get; set; }
}
