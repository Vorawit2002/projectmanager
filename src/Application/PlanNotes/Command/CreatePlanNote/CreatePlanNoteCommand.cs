using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.PlanNotes.Command.CreatePlanNote;
public class CreatePlanNoteCommand : IRequest<bool>
{
    public Guid ActivityPlanId { get; set; }
    public string Summary { get; set; } = default!;
    public string? ToDoNext { get; set; }
    public string? Remarks { get; set; }
}
public class CreatePlanNoteCommandHandler : IRequestHandler<CreatePlanNoteCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public CreatePlanNoteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(CreatePlanNoteCommand request, CancellationToken cancellationToken)
    {
        var planNotes = new PlanNote();
        planNotes.ActivityPlanId = request.ActivityPlanId;
        planNotes.Summary = request.Summary;
        planNotes.ToDoNext = request.ToDoNext;
        planNotes.Remarks = request.Remarks;
        await _context.PlanNotes.AddAsync(planNotes);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
