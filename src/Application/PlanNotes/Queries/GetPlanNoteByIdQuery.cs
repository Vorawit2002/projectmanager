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
public record GetPlanNoteByIdQuery (Guid Id): IRequest<PlanNoteDto>;
public class GetPlanNoteByIdQueryHandler : IRequestHandler<GetPlanNoteByIdQuery, PlanNoteDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPlanNoteByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<PlanNoteDto> Handle(GetPlanNoteByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.PlanNotes.FirstOrDefaultAsync(x => x.Id == request.Id);
        Guard.Against.NotFound(request.Id, entity);

        var planNotesDto = _mapper.Map<PlanNote, PlanNoteDto>(entity);

        return planNotesDto;
    }
}
