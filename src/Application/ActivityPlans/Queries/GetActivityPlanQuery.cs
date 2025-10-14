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

namespace ProjectManagement.Application.ActivityPlans.Queries;

public record GetActivityPlanQuery : IRequest<IEnumerable<ActivityPlanDto>>;

public class GetActivityPlanQueryHandler : IRequestHandler<GetActivityPlanQuery, IEnumerable<ActivityPlanDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetActivityPlanQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<ActivityPlanDto>> Handle(GetActivityPlanQuery request, CancellationToken cancellationToken)
    {
        return await _context.ActivityPlans
            .Include(x => x.Employees)
            .Include(x => x.Projects)
            .Include(x => x.Organizations)
            .ProjectTo<ActivityPlanDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
         
    }
}
