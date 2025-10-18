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
    private readonly IUser _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IDataFilterService _dataFilterService;

    public GetActivityPlanQueryHandler(
        IApplicationDbContext context, 
        IMapper mapper,
        IUser currentUser,
        IIdentityService identityService,
        IDataFilterService dataFilterService)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
        _identityService = identityService;
        _dataFilterService = dataFilterService;
    }
    
    public async Task<IEnumerable<ActivityPlanDto>> Handle(GetActivityPlanQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id ?? throw new UnauthorizedAccessException();
        var roles = (await _identityService.GetUserRolesAsync(userId)).ToArray();
        
        var query = _context.ActivityPlans
            .Include(x => x.Employees)
            .Include(x => x.Projects)
            .Include(x => x.Organizations)
            .AsQueryable();
            
        // Apply role-based filtering
        query = await _dataFilterService.ApplyRoleBasedFilterAsync(query, userId, roles);
        
        return await query
            .ProjectTo<ActivityPlanDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
