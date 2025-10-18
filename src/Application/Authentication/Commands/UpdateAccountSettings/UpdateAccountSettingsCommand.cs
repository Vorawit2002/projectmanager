using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;

namespace ProjectManagement.Application.Authentication.Commands.UpdateAccountSettings;

public record UpdateAccountSettingsCommand : IRequest<Result>
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? TitleName { get; init; }
    public string? Position { get; init; }
    public string? Phone { get; init; }
    public Guid? DepartmentId { get; init; }
}

public class UpdateAccountSettingsCommandValidator : AbstractValidator<UpdateAccountSettingsCommand>
{
    public UpdateAccountSettingsCommandValidator()
    {
        RuleFor(v => v.FirstName)
            .MaximumLength(100).WithMessage("ชื่อต้องมีความยาวไม่เกิน 100 ตัวอักษร");

        RuleFor(v => v.LastName)
            .MaximumLength(100).WithMessage("นามสกุลต้องมีความยาวไม่เกิน 100 ตัวอักษร");

        RuleFor(v => v.TitleName)
            .MaximumLength(50).WithMessage("คำนำหน้าต้องมีความยาวไม่เกิน 50 ตัวอักษร");

        RuleFor(v => v.Position)
            .MaximumLength(100).WithMessage("ตำแหน่งต้องมีความยาวไม่เกิน 100 ตัวอักษร");

        RuleFor(v => v.Phone)
            .MaximumLength(20).WithMessage("เบอร์โทรศัพท์ต้องมีความยาวไม่เกิน 20 ตัวอักษร")
            .Matches(@"^[\d\-\+\(\)\s]*$").WithMessage("รูปแบบเบอร์โทรศัพท์ไม่ถูกต้อง")
            .When(v => !string.IsNullOrEmpty(v.Phone));
    }
}

public class UpdateAccountSettingsCommandHandler : IRequestHandler<UpdateAccountSettingsCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;

    public UpdateAccountSettingsCommandHandler(
        IApplicationDbContext context,
        IUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(UpdateAccountSettingsCommand request, CancellationToken cancellationToken)
    {
        // Get current user ID
        var userId = _currentUser.Id;
        
        if (string.IsNullOrEmpty(userId))
        {
            return Result.Failure(new[] { "ไม่พบข้อมูลผู้ใช้งาน" });
        }

        // Find employee record by UserId
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken);

        if (employee == null)
        {
            return Result.Failure(new[] { "ไม่พบข้อมูลพนักงาน" });
        }

        // Validate department exists if DepartmentId is provided
        if (request.DepartmentId.HasValue)
        {
            var departmentExists = await _context.Departments
                .AnyAsync(d => d.Id == request.DepartmentId.Value, cancellationToken);

            if (!departmentExists)
            {
                return Result.Failure(new[] { "ไม่พบข้อมูลแผนกที่เลือก" });
            }
        }

        // Update employee fields
        employee.FirstName = request.FirstName;
        employee.LastName = request.LastName;
        employee.TitleName = request.TitleName;
        employee.Position = request.Position;
        employee.Phone = request.Phone;
        employee.DepartmentId = request.DepartmentId;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new[] { $"เกิดข้อผิดพลาดในการบันทึกข้อมูล: {ex.Message}" });
        }
    }
}
