using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace ProjectManagement.Application.FileAttachments.Commands;
public class UploadAttachmentCommand : IRequest<Attachment>
{
    public Stream FileStream { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public string subdirectory { get; set; } = default!;
    public string subindirectory { get; set; } = default!;
}

public class UploadAttachmentCommandHandler : IRequestHandler<UploadAttachmentCommand, Attachment>
{
    private readonly IConfiguration _configuration;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMinIOService _minIOService;
    private readonly IStoragePathService _storagePathService;

    public UploadAttachmentCommandHandler(IConfiguration configuration, IApplicationDbContext dbContext, IMinIOService minIOService, IStoragePathService storagePathService)
    {
        _configuration = configuration;
        _dbContext = dbContext;
        _minIOService = minIOService;
        _storagePathService = storagePathService;
    }

    public async Task<Attachment> Handle(UploadAttachmentCommand request, CancellationToken cancellationToken)
    {
        if (request.FileStream == null || request.FileStream.Length == 0)
            Guard.Against.NullOrEmpty("File content is null");
        string BusketOriginalName = "crmfile";

        using var memoryStreamOriginal = new MemoryStream();
        await request.FileStream!.CopyToAsync(memoryStreamOriginal);
        memoryStreamOriginal.Position = 0;
        using var memoryStreamThumbnail = new MemoryStream(memoryStreamOriginal.ToArray());
        var fileAttachment = new Attachment();

        if (request.FileStream != null)
        {
            //path
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.FileName)}";
            var subFolder = _storagePathService.GeneratePrefixPath(request.subdirectory);
            var filePath = $"{subFolder}/{request.subindirectory}/{uniqueFileName}";

            string originalFileName = request.FileName;
            await _minIOService.Upload(memoryStreamOriginal, BusketOriginalName, filePath);
            fileAttachment.BucketOriginalName = BusketOriginalName;
            fileAttachment.BucketOriginalPath = filePath;
            fileAttachment.NameFile = originalFileName;
            fileAttachment.PathFile = filePath;
            //fileAttachment.PrefixFilePath = subFolder;
            fileAttachment.FileSize = request.FileStream.Length;
            fileAttachment.FileExtension = Path.GetExtension(request.FileName);

            _dbContext.Attachments.Add(fileAttachment);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return fileAttachment;
        }
        return default!;
    }
}
