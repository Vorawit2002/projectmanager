using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.ActivityPlans.Queries;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.OrganizationContacts.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Attachments.Queries;
public record GetAttachmentByIdQuery(Guid Id): IRequest<AttachmentDto>;
public class GetAttachmentByIdQueryHandler : IRequestHandler<GetAttachmentByIdQuery, AttachmentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMinIOService _minIOService;
    public GetAttachmentByIdQueryHandler(IApplicationDbContext context, IMapper mapper, IMinIOService minIOService)
    {
        _context = context;
        _mapper = mapper;
        _minIOService = minIOService;
    }
    public async Task<AttachmentDto> Handle(GetAttachmentByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Attachments
            .FirstOrDefaultAsync(l => l.Id == request.Id);
        Guard.Against.NotFound(request.Id, entity);
        var attachmentDto = _mapper.Map<Attachment, AttachmentDto>(entity);
        byte[] byteArray = await _minIOService.DownloadToByteArray(entity.BucketOriginalName, entity.BucketOriginalPath);
        string base64String = Convert.ToBase64String(byteArray);
        attachmentDto.ThumbnailBase64 = base64String;
        return attachmentDto;
    }
}

