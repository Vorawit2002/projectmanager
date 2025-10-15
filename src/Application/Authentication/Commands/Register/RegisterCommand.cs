using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;

namespace ProjectManagement.Application.Authentication.Commands.Register;

public record RegisterCommand : IRequest<Result>
{
    public string Email { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
}

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(v => v.Email)
            .NotEmpty().WithMessage("กรุณากรอกอีเมล")
            .EmailAddress().WithMessage("รูปแบบอีเมลไม่ถูกต้อง");

        RuleFor(v => v.Username)
            .NotEmpty().WithMessage("กรุณากรอกชื่อผู้ใช้")
            .MinimumLength(3).WithMessage("ชื่อผู้ใช้ต้องมีความยาวอย่างน้อย 3 ตัวอักษร")
            .MaximumLength(50).WithMessage("ชื่อผู้ใช้ต้องมีความยาวไม่เกิน 50 ตัวอักษร");

        RuleFor(v => v.Password)
            .NotEmpty().WithMessage("กรุณากรอกรหัสผ่าน")
            .MinimumLength(6).WithMessage("รหัสผ่านต้องมีความยาวอย่างน้อย 6 ตัวอักษร");

        RuleFor(v => v.ConfirmPassword)
            .NotEmpty().WithMessage("กรุณายืนยันรหัสผ่าน")
            .Equal(v => v.Password).WithMessage("รหัสผ่านไม่ตรงกัน");

        RuleFor(v => v.FirstName)
            .NotEmpty().WithMessage("กรุณากรอกชื่อ");

        RuleFor(v => v.LastName)
            .NotEmpty().WithMessage("กรุณากรอกนามสกุล");
    }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result>
{
    private readonly IIdentityService _identityService;

    public RegisterCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var registerDto = new RegisterDto
        {
            Email = request.Email,
            Username = request.Username,
            Password = request.Password,
            ConfirmPassword = request.ConfirmPassword,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var (result, userId) = await _identityService.RegisterUserAsync(registerDto);

        return result;
    }
}
