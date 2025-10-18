using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;

namespace ProjectManagement.Application.Users.Commands.RemoveRole;

public record RemoveRoleCommand : IRequest<Result>
{
    public string UserId { get; init; } = string.Empty;
    public string RoleName { get; init; } = string.Empty;
}

public class RemoveRoleCommandValidator : AbstractValidator<RemoveRoleCommand>
{
    public RemoveRoleCommandValidator()
    {
        RuleFor(v => v.UserId)
            .NotEmpty().WithMessage("กรุณาระบุ User ID");

        RuleFor(v => v.RoleName)
            .NotEmpty().WithMessage("กรุณาระบุชื่อ Role");
    }
}

public class RemoveRoleCommandHandler : IRequestHandler<RemoveRoleCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;

    public RemoveRoleCommandHandler(
        IIdentityService identityService,
        IApplicationDbContext context)
    {
        _identityService = identityService;
        _context = context;
    }

    public async Task<Result> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
    {
        // Remove role using Identity Service
        var result = await _identityService.RemoveRoleAsync(request.UserId, request.RoleName);

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
