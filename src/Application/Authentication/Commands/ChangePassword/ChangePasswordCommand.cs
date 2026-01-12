using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;

namespace ProjectManagement.Application.Authentication.Commands.ChangePassword;

public record ChangePasswordCommand : IRequest<Result>
{
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
    public string ConfirmNewPassword { get; init; } = string.Empty;
}

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(v => v.CurrentPassword)
            .NotEmpty().WithMessage("กรุณากรอกรหัสผ่านปัจจุบัน");

        RuleFor(v => v.NewPassword)
            .NotEmpty().WithMessage("กรุณากรอกรหัสผ่านใหม่")
            .MinimumLength(6).WithMessage("รหัสผ่านใหม่ต้องมีความยาวอย่างน้อย 6 ตัวอักษร")
            .NotEqual(v => v.CurrentPassword).WithMessage("รหัสผ่านใหม่ต้องไม่เหมือนกับรหัสผ่านปัจจุบัน");

        RuleFor(v => v.ConfirmNewPassword)
            .NotEmpty().WithMessage("กรุณายืนยันรหัสผ่านใหม่")
            .Equal(v => v.NewPassword).WithMessage("รหัสผ่านใหม่ไม่ตรงกัน");
    }
}

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly IUser _currentUser;

    public ChangePasswordCommandHandler(
        IIdentityService identityService,
        IUser currentUser)
    {
        _identityService = identityService;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id;

        if (string.IsNullOrEmpty(userId))
        {
            return Result.Failure(new[] { "ไม่พบข้อมูลผู้ใช้งาน" });
        }

        var result = await _identityService.ChangePasswordAsync(
            userId,
            request.CurrentPassword,
            request.NewPassword);

        return result;
    }
}
