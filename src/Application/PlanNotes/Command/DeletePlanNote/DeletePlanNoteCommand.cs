using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.PlanNotes.Command.DeletePlanNote;
public record DeletePlanNoteCommand (Guid Id): IRequest<bool>;
public class DeletePlanNoteCommandHandler : IRequestHandler<DeletePlanNoteCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeletePlanNoteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(DeletePlanNoteCommand request, CancellationToken cancellationToken)
    {
        var planNotes = await _context.PlanNotes.FirstOrDefaultAsync(x => x.Id == request.Id);
        Guard.Against.NotFound(request.Id, planNotes);
        _context.PlanNotes.Remove(planNotes);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
