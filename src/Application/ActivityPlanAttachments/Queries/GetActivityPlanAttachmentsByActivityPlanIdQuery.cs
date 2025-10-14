using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.ActivityPlanAttachments.Queries;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.ActivityPlanAttachments.Queries;
public class GetActivityPlanAttachmentsByActivityPlanIdQuery : IRequest<List<ActivityPlanAttachmentDto>>
{
    public Guid ActivityPlanId { get; set; }
}
public class GetActivityPlanAttachmentsByActivityPlanIdQueryHandle : IRequestHandler<GetActivityPlanAttachmentsByActivityPlanIdQuery, List<ActivityPlanAttachmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMinIOService _minIOService;

    public GetActivityPlanAttachmentsByActivityPlanIdQueryHandle(IApplicationDbContext context, IMapper mapper,IMinIOService minIOService)
    {
        _context = context;
        _mapper = mapper;
        _minIOService = minIOService;
    }
    public async Task<List<ActivityPlanAttachmentDto>> Handle(GetActivityPlanAttachmentsByActivityPlanIdQuery request, CancellationToken cancellationToken)
    {
        var activityPlansAttachmentDtos = await _context.ActivityPlanAttachments.Include(x => x.Attachments).Include(x => x.ActivityPlans)
            .Where(l => l.ActivityPlanId == request.ActivityPlanId)
            .ProjectTo<ActivityPlanAttachmentDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        foreach(var activityPlansAttachment in activityPlansAttachmentDtos)
        {
            byte[] byteArray = await _minIOService.DownloadToByteArray(activityPlansAttachment.Attachments.BucketOriginalName, activityPlansAttachment.Attachments.BucketOriginalPath);
            string base64String = Convert.ToBase64String(byteArray);
            activityPlansAttachment.ThumbnailBase64 = base64String;
        }
        return activityPlansAttachmentDtos;
    }
}
