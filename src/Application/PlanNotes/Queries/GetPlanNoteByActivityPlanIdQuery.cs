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
public record GetPlanNoteByActivityPlanIdQuery (Guid Id): IRequest<PlanNoteDto>;
public class GetPlanNoteByActivityPlanIdQueryHandler : IRequestHandler<GetPlanNoteByActivityPlanIdQuery, PlanNoteDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPlanNoteByActivityPlanIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<PlanNoteDto> Handle(GetPlanNoteByActivityPlanIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.PlanNotes.FirstOrDefaultAsync(x => x.ActivityPlanId == request.Id);
        if (entity != null)
        {
            var planNotesDto = _mapper.Map<PlanNote, PlanNoteDto>(entity);

            return planNotesDto;
        }
        else
        {
            return new PlanNoteDto();
        }
    }
}
