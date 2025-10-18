using Microsoft.AspNetCore.Http;
using ProjectManagement.Application.Common.Models;

namespace ProjectManagement.Application.Users.Commands.UploadProfileImage;

public class UploadProfileImageCommand : IRequest<Result<string>>
{
    public IFormFile ImageFile { get; set; } = default!;
    public string UserId { get; set; } = default!;
}
