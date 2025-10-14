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

public record GetActivityPlanByIdQuery(Guid Id) : IRequest<ActivityPlanDto>;

public class GetActivityPlanByIdQueryHandler : IRequestHandler<GetActivityPlanByIdQuery, ActivityPlanDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetActivityPlanByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ActivityPlanDto> Handle(GetActivityPlanByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.ActivityPlans
            .Include(x => x.Employees)
            .ThenInclude(x => x!.Departments)
            .Include(x => x.Projects)
            .Include(x => x.Organizations)
            .FirstOrDefaultAsync(l => l.Id == request.Id);
        Guard.Against.NotFound(request.Id, entity);
        var activityPlanDto = _mapper.Map<ActivityPlan, ActivityPlanDto>(entity);
        return activityPlanDto;
    }
}
