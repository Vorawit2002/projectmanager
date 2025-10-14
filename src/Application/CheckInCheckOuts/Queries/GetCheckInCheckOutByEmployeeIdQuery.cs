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
public record GetCheckInCheckOutByEmployeeIdQuery (Guid Id) : IRequest<IList<CheckInCheckOutDto>>;

public class GetCheckInCheckOutByEmployeeIdQueryHandler : IRequestHandler<GetCheckInCheckOutByEmployeeIdQuery, IList<CheckInCheckOutDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMinIOService _minIOService;
    public GetCheckInCheckOutByEmployeeIdQueryHandler(IApplicationDbContext context, IMapper mapper, IMinIOService minIOService)
    {
        _context = context;
        _mapper = mapper;
        _minIOService = minIOService;
    }
    public async Task<IList<CheckInCheckOutDto>> Handle(GetCheckInCheckOutByEmployeeIdQuery request, CancellationToken cancellationToken)
    {
        var checkInCheckOutDtos = await _context.CheckInCheckOuts
            .Include(x => x.Projects)
            .Include(x => x.Organizations)
            .Include(x => x.Employees)
            .Include(x => x.CheckInCheckOutAttachments)
            .ThenInclude(x => x.Attachments)
            .Where(l => l.EmployeeId == request.Id)
            .OrderByDescending(x => x.Created)
             .ProjectTo<CheckInCheckOutDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        foreach (var checkInCheckOutAttachment in checkInCheckOutDtos)
        {
            if(checkInCheckOutAttachment.CheckInCheckOutAttachments != null)
            {
                foreach (var attachment in checkInCheckOutAttachment.CheckInCheckOutAttachments)
                {
                    byte[] byteArray = await _minIOService.DownloadToByteArray(attachment.Attachments.BucketOriginalName, attachment.Attachments.BucketOriginalPath);
                    string base64String = Convert.ToBase64String(byteArray);
                    if (attachment.Type == Domain.Enums.CheckInCheckOutType.CheckIn)
                    {
                        checkInCheckOutAttachment.CheckInImage = base64String;
                    }
                    else
                    {
                        checkInCheckOutAttachment.CheckOutImage = base64String;

                    }
                }
            }
        }
        return checkInCheckOutDtos;
    }
}
