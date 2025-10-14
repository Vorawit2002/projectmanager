using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Organizations.Queries;
using ProjectManagement.Application.PlanNotes.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.PlanNotes.Command.DeletePlanNote;
public record GetPlanNoteQuery: IRequest<IEnumerable<PlanNoteDto>>;
public class GetPlanNoteQueryHandler : IRequestHandler<GetPlanNoteQuery, IEnumerable<PlanNoteDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPlanNoteQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<PlanNoteDto>> Handle(GetPlanNoteQuery request, CancellationToken cancellationToken)
    {
        return await _context.PlanNotes
           .ProjectTo<PlanNoteDto>(_mapper.ConfigurationProvider)
           .ToListAsync();
    }
}
