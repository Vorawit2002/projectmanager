using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.EventTypes.Command.UpdateEventType;
public record DeleteEventTypeCommand (Guid Id) : IRequest<bool>;

public class DeleteEventTypeCommandHandler : IRequestHandler<DeleteEventTypeCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteEventTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(DeleteEventTypeCommand request, CancellationToken cancellationToken)
    {
        var eventTypes = await _context.EventTypes.FirstOrDefaultAsync(x => x.Id == request.Id);
        Guard.Against.NotFound(request.Id, eventTypes);
         _context.EventTypes.Remove(eventTypes);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
