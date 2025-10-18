using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace ProjectManagement.Application.Users.Commands.UploadProfileImage;

public class UploadProfileImageCommandValidator : AbstractValidator<UploadProfileImageCommand>
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public UploadProfileImageCommandValidator()
    {
        RuleFor(v => v.ImageFile)
            .NotNull()
            .WithMessage("กรุณาเลือกไฟล์รูปภาพ");

        RuleFor(v => v.ImageFile)
            .Must(BeValidFileType!)
            .When(v => v.ImageFile != null)
            .WithMessage("กรุณาเลือกไฟล์รูปภาพประเภท JPEG, PNG, JPG หรือ GIF");

        RuleFor(v => v.ImageFile)
            .Must(BeValidFileSize!)
            .When(v => v.ImageFile != null)
            .WithMessage("ขนาดไฟล์ต้องไม่เกิน 5MB");

        RuleFor(v => v.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");
    }

    private bool BeValidFileType(IFormFile file)
    {
        if (file == null) return false;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }

    private bool BeValidFileSize(IFormFile file)
    {
        if (file == null) return false;

        return file.Length <= MaxFileSize;
    }
}
