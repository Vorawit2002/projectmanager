namespace ProjectManagement.Application.Users.Queries.GetAccountSettings;

public class AccountSettingsDto
{
    public string UserId { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    
    // Personal Information
    public string? TitleName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    // Contact Information
    public string? Phone { get; set; }
    
    // Position and Department
    public string? Position { get; set; }
    public string? Department { get; set; }
    public Guid? DepartmentId { get; set; }
    
    // Profile Image
    public string? ImageProfile { get; set; }
    
    // User Status
    public List<string> Roles { get; set; } = new();
    public bool IsActive { get; set; }
}
