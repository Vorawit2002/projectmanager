using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.EventTypes.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.EventTypes.Command.UpdateEventType;
public record GetEventTypeQuery : IRequest<IEnumerable<EventTypeDto>>;

public class GetEventTypeQueryHandler : IRequestHandler<GetEventTypeQuery, IEnumerable<EventTypeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetEventTypeQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<EventTypeDto>> Handle(GetEventTypeQuery request, CancellationToken cancellationToken)
    {
        return await _context.EventTypes
            .ProjectTo<EventTypeDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}
