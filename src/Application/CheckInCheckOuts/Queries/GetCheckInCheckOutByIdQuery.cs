using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.ActivityPlanAttachments.Queries;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Departments.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.CheckInCheckOuts.Queries;
public record GetCheckInCheckOutByIdQuery (Guid Id) : IRequest<CheckInCheckOutDto>;

public class GetCheckInCheckOutByIdQueryHandler : IRequestHandler<GetCheckInCheckOutByIdQuery, CheckInCheckOutDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMinIOService _minIOService;
    public GetCheckInCheckOutByIdQueryHandler(IApplicationDbContext context, IMapper mapper, IMinIOService minIOService)
    {
        _context = context;
        _mapper = mapper;
        _minIOService = minIOService;
    }
    public async Task<CheckInCheckOutDto> Handle(GetCheckInCheckOutByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.CheckInCheckOuts
            .Include(x => x.Projects)
            .Include(x => x.Organizations)
            .Include(x => x.Employees)
            .Include(x => x.CheckInCheckOutAttachments)
            .ThenInclude(x => x.Attachments)
            .OrderByDescending(x => x.Created)
            .FirstOrDefaultAsync(l => l.Id == request.Id);

        Guard.Against.NotFound(request.Id, entity);
        var checkInCheckOutDto = _mapper.Map<CheckInCheckOut, CheckInCheckOutDto>(entity);
        foreach (var attachment in checkInCheckOutDto.CheckInCheckOutAttachments)
        {
            byte[] byteArray = await _minIOService.DownloadToByteArray(attachment.Attachments.BucketOriginalName, attachment.Attachments.BucketOriginalPath);
            string base64String = Convert.ToBase64String(byteArray);
            if (attachment.Type == Domain.Enums.CheckInCheckOutType.CheckIn)
            {
                checkInCheckOutDto.CheckInImage = base64String;
            }
            else
            {
                checkInCheckOutDto.CheckOutImage = base64String;

            }
        }
        return checkInCheckOutDto;
    }
}
