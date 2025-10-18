using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Domain.Constants;

namespace ProjectManagement.Application.Users.Commands.AssignRole;

public record AssignRoleCommand : IRequest<Result>
{
    public string UserId { get; init; } = string.Empty;
    public string RoleName { get; init; } = string.Empty;
}

public class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
{
    public AssignRoleCommandValidator()
    {
        RuleFor(v => v.UserId)
            .NotEmpty().WithMessage("กรุณาระบุ User ID");

        RuleFor(v => v.RoleName)
            .NotEmpty().WithMessage("กรุณาระบุชื่อ Role")
            .Must(BeValidRole).WithMessage("Role ที่เลือกไม่ถูกต้อง กรุณาเลือก Admin, Manager, User หรือ Viewer");
    }

    private bool BeValidRole(string roleName)
    {
        var validRoles = new[]
        {
            Roles.Admin,
            Roles.Manager,
            Roles.User,
            Roles.Viewer
        };

        return validRoles.Contains(roleName, StringComparer.OrdinalIgnoreCase);
    }
}

public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;

    public AssignRoleCommandHandler(
        IIdentityService identityService,
        IApplicationDbContext context)
    {
        _identityService = identityService;
        _context = context;
    }

    public async Task<Result> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        // Assign role using Identity Service
        var result = await _identityService.AssignRoleAsync(request.UserId, request.RoleName);

        if (!result.Succeeded)
        {
            return result;
        }

        // Update Employee.Roles field if Employee record exists
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.UserId == request.UserId, cancellationToken);

        if (employee != null)
        {
            // Get all current roles for the user
            var userRoles = await _identityService.GetUserRolesAsync(request.UserId);
            
            // Update the Roles field with comma-separated role names
            employee.Roles = string.Join(", ", userRoles);
            
            await _context.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
