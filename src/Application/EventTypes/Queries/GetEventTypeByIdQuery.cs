using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.EventTypes.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.EventTypes.Command.UpdateEventType;
public record GetEventTypeByIdQuery(Guid Id) : IRequest<EventTypeDto>;

public class GetEventTypeByIdQueryHandler : IRequestHandler<GetEventTypeByIdQuery, EventTypeDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetEventTypeByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<EventTypeDto> Handle(GetEventTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.EventTypes.FirstOrDefaultAsync(x => x.Id == request.Id);

        Guard.Against.NotFound(request.Id, entity);

        var eventTypeDto = _mapper.Map<EventType, EventTypeDto>(entity);
        return eventTypeDto;
    }
}
