using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;

namespace ProjectManagement.Application.Users.Commands.UploadProfileImage;

public class UploadProfileImageCommandHandler : IRequestHandler<UploadProfileImageCommand, Result<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMinIOService _minioService;
    private readonly IIdentityService _identityService;
    private readonly ILogger<UploadProfileImageCommandHandler> _logger;

    public UploadProfileImageCommandHandler(
        IApplicationDbContext context,
        IMinIOService minioService,
        IIdentityService identityService,
        ILogger<UploadProfileImageCommandHandler> logger)
    {
        _context = context;
        _minioService = minioService;
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(UploadProfileImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("=== Starting profile image upload for user {UserId} ===", request.UserId);

            // Get user from Identity
            var user = await _identityService.GetUserByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", request.UserId);
                return Result<string>.Failure(new[] { "ไม่พบข้อมูลผู้ใช้" });
            }

            _logger.LogInformation("User found: {Username}", user.UserName);

            // Find the employee record for this user
            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.UserId == request.UserId, cancellationToken);

            _logger.LogInformation("Employee record: {HasEmployee}", employee != null ? "Found" : "Not Found");

            // Validate file
            if (request.ImageFile == null || request.ImageFile.Length == 0)
            {
                _logger.LogWarning("No image file provided");
                return Result<string>.Failure(new[] { "ไม่พบไฟล์รูปภาพ" });
            }

            _logger.LogInformation("File info - Name: {FileName}, Size: {FileSize} bytes, ContentType: {ContentType}", 
                request.ImageFile.FileName, request.ImageFile.Length, request.ImageFile.ContentType);

            // Use Base64 storage (simple and reliable)
            string imageUrl;
            try
            {
                _logger.LogInformation("Converting image to Base64...");
                
                using var memoryStream = new MemoryStream();
                await request.ImageFile.CopyToAsync(memoryStream, cancellationToken);
                var imageBytes = memoryStream.ToArray();
                var base64String = Convert.ToBase64String(imageBytes);
                var mimeType = request.ImageFile.ContentType ?? "image/jpeg";
                imageUrl = $"data:{mimeType};base64,{base64String}";

                _logger.LogInformation("Image converted to Base64 successfully. Size: {Size} bytes, Base64 length: {Length}", 
                    imageBytes.Length, base64String.Length);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to convert image to Base64");
                return Result<string>.Failure(new[] { $"ไม่สามารถประมวลผลรูปภาพได้: {ex.Message}" });
            }

            // Update employee record with image URL if exists
            if (employee != null)
            {
                _logger.LogInformation("Updating Employee.ImageProfile...");
                
                // Use ExecuteUpdateAsync to avoid tracking issues
                await _context.Employees
                    .Where(e => e.Id == employee.Id)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(e => e.ImageProfile, imageUrl), 
                        cancellationToken);
                
                _logger.LogInformation("Employee.ImageProfile updated successfully");
            }

            // Update ApplicationUser.ImageProfile
            _logger.LogInformation("Updating ApplicationUser.ImageProfile...");
            user.ImageProfile = imageUrl;
            
            var updateResult = await _identityService.UpdateUserAsync(user);
            if (!updateResult.Succeeded)
            {
                _logger.LogError("Failed to update user: {Errors}", string.Join(", ", updateResult.Errors));
                return Result<string>.Failure(new[] { "ไม่สามารถบันทึกข้อมูลผู้ใช้ได้" });
            }

            // No need to call SaveChangesAsync since ExecuteUpdateAsync already saves
            _logger.LogInformation("All changes saved successfully");

            _logger.LogInformation("=== Profile image uploaded successfully for user {UserId} ===", request.UserId);

            return Result<string>.Success(imageUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "=== ERROR uploading profile image for user {UserId} ===", request.UserId);
            _logger.LogError(ex, "Exception details: {Message}", ex.Message);
            _logger.LogError(ex, "Stack trace: {StackTrace}", ex.StackTrace);
            return Result<string>.Failure(new[] { $"เกิดข้อผิดพลาด: {ex.Message}" });
        }
    }
}
