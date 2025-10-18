using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;

namespace ProjectManagement.Application.Users.Commands.UpdateAccountSettings;

public record UpdateAccountSettingsCommand : IRequest<Result<bool>>
{
    public string? TitleName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Position { get; set; }
    public string? Phone { get; set; }
    public Guid? DepartmentId { get; set; }
}

public class UpdateAccountSettingsCommandHandler : IRequestHandler<UpdateAccountSettingsCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;

    public UpdateAccountSettingsCommandHandler(IApplicationDbContext context, IUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<bool>> Handle(UpdateAccountSettingsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUser.Id;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Result<bool>.Failure(new[] { "User not authenticated" });
            }

            // Find employee record
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken);

            if (employee == null)
            {
                return Result<bool>.Failure(new[] { "Employee record not found" });
            }

            // Update employee fields
            if (!string.IsNullOrEmpty(request.TitleName))
                employee.TitleName = request.TitleName;
            
            if (!string.IsNullOrEmpty(request.FirstName))
                employee.FirstName = request.FirstName;
            
            if (!string.IsNullOrEmpty(request.LastName))
                employee.LastName = request.LastName;
            
            if (!string.IsNullOrEmpty(request.Position))
                employee.Position = request.Position;
            
            if (!string.IsNullOrEmpty(request.Phone))
                employee.Phone = request.Phone;
            
            if (request.DepartmentId.HasValue)
                employee.DepartmentId = request.DepartmentId.Value;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(new[] { $"Error updating account settings: {ex.Message}" });
        }
    }
}
