using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Mappings;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Organizations.Queries;
using ProjectManagement.Application.PlanNotes.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Organizations.Queries;

public class GetPlannteWithPaginationQuery: IRequest<PaginatedList<PlanNoteDto>>
{
     public int PageNumber { get; init; } = 1;
     public int PageSize { get; init; } = 10;
}
public class GetPlannteWithPaginationQueryHandler : IRequestHandler<GetPlannteWithPaginationQuery, PaginatedList<PlanNoteDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPlannteWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<PaginatedList<PlanNoteDto>> Handle(GetPlannteWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return await _context.PlanNotes
                .OrderBy(x => x.ActivityPlanId)
                .ProjectTo<PlanNoteDto>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize);

    }
}
