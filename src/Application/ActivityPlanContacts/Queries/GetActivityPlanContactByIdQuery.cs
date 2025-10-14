using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Departments.Queries;
using ProjectManagement.Application.Employees.Queries;
using ProjectManagement.Application.Organizations.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.ActivityPlanContacts.Queries;

public record GetActivityPlanContactByIdQuery(Guid Id) : IRequest<ActivityPlanContactDto>;

public class GetActivityPlanContactByIdQueryHandler : IRequestHandler<GetActivityPlanContactByIdQuery, ActivityPlanContactDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetActivityPlanContactByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ActivityPlanContactDto> Handle(GetActivityPlanContactByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.ActivityPlanContacts
            .FirstOrDefaultAsync(l => l.Id == request.Id);
        Guard.Against.NotFound(request.Id, entity);
        var activityPlanContactDto = _mapper.Map<ActivityPlanContact, ActivityPlanContactDto>(entity);
        return activityPlanContactDto;
    }
}
