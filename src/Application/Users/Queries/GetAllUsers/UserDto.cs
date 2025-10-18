namespace ProjectManagement.Application.Users.Queries.GetAllUsers;

public class UserDto
{
    public string UserId { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ImageProfile { get; set; }
    public string? Department { get; set; }
    public Guid? DepartmentId { get; set; }
    public List<string> Roles { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime? LastLoginDate { get; set; }
}
