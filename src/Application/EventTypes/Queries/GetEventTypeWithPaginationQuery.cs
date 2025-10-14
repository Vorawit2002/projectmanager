using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Mappings;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Employees.Queries;
using ProjectManagement.Application.EventTypes.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.EventTypes.Command.UpdateEventType;
public class GetEventTypeWithPaginationQuery: IRequest<PaginatedList<EventTypeDto>>
{
    public string? Search { get; set; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetEventTypeWithPaginationQueryHandler : IRequestHandler<GetEventTypeWithPaginationQuery, PaginatedList<EventTypeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetEventTypeWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<PaginatedList<EventTypeDto>> Handle(GetEventTypeWithPaginationQuery request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request.Search))
        {
            return await _context.EventTypes
                        .Where(x => x.Name.Contains(request.Search))
                        .OrderBy(x => x.Name)
                        .ProjectTo<EventTypeDto>(_mapper.ConfigurationProvider)
                        .PaginatedListAsync(request.PageNumber, request.PageSize);
        }
        else
        {
            return await _context.EventTypes
                       .OrderBy(x => x.Name)
                       .ProjectTo<EventTypeDto>(_mapper.ConfigurationProvider)
                       .PaginatedListAsync(request.PageNumber, request.PageSize);
        }
    }
}
