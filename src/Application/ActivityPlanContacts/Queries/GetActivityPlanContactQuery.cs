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

public record GetActivityPlanContactQuery: IRequest<IEnumerable<ActivityPlanContactDto>>;

public class GetActivityPlanContactQueryHandler : IRequestHandler<GetActivityPlanContactQuery, IEnumerable<ActivityPlanContactDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetActivityPlanContactQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<ActivityPlanContactDto>> Handle(GetActivityPlanContactQuery request, CancellationToken cancellationToken)
    {
        return await _context.ActivityPlanContacts
            .ProjectTo<ActivityPlanContactDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
         
    }
}
