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

public class GetActivityPlanContactByActivityPlanIdQuery : IRequest<IEnumerable<ActivityPlanContactDto>>
{
    public Guid ActivityPlanId { get; set; }
}

public class GetActivityPlanContactByActivityPlanIdQueryHandler : IRequestHandler<GetActivityPlanContactByActivityPlanIdQuery, IEnumerable<ActivityPlanContactDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetActivityPlanContactByActivityPlanIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<ActivityPlanContactDto>> Handle(GetActivityPlanContactByActivityPlanIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.ActivityPlanContacts.Where(x => x.ActivityPlanId == request.ActivityPlanId)
            .ProjectTo<ActivityPlanContactDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
         
    }
}
