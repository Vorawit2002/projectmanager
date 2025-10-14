using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Organizations.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.OrganizationContacts.Queries;

public record GetOrganizationContactByIdQuery(Guid Id) : IRequest<OrganizationContactDto>;

public class GetOrganizationContactByIdQueryHandler : IRequestHandler<GetOrganizationContactByIdQuery, OrganizationContactDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMinIOService _minIOService;
    public GetOrganizationContactByIdQueryHandler(IApplicationDbContext context, IMapper mapper, IMinIOService minIOService)
    {
        _context = context;
        _mapper = mapper;
        _minIOService = minIOService;
    }
    public async Task<OrganizationContactDto> Handle(GetOrganizationContactByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.OrganizationContacts
            .Include(x => x.Attachments)
            .FirstOrDefaultAsync(l => l.Id == request.Id);

        Guard.Against.NotFound(request.Id, entity);

        var organizationContactDto = _mapper.Map<OrganizationContact, OrganizationContactDto>(entity);
        if (organizationContactDto.AttachmentId != Guid.Empty && organizationContactDto.AttachmentId != null)
        {
            byte[] byteArray = await _minIOService.DownloadToByteArray(entity.Attachments!.BucketOriginalName, entity.Attachments.BucketOriginalPath);
            string base64String = Convert.ToBase64String(byteArray);
            organizationContactDto.ImageProfile = base64String;
        }
        return organizationContactDto;
    }
}
