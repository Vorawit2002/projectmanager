using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.ActivityPlans.Queries;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Mappings;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Employees.Queries;
using ProjectManagement.Application.Organizations.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.ActivityPlanContacts.Queries;

public class GetActivityPlanContactWithPaginationQuery : IRequest<PaginatedList<ActivityPlanContactDto>>
{
     public int PageNumber { get; init; } = 1;
     public int PageSize { get; init; } = 10;
}
public class GetActivityPlanContactWithPaginationQueryHandler : IRequestHandler<GetActivityPlanContactWithPaginationQuery, PaginatedList<ActivityPlanContactDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetActivityPlanContactWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<PaginatedList<ActivityPlanContactDto>> Handle(GetActivityPlanContactWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return await _context.ActivityPlanContacts
                .OrderBy(x => x.ActivityPlanId)
                .ProjectTo<ActivityPlanContactDto>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize);

    }
}
