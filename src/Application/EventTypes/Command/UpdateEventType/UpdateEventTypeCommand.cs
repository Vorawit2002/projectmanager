using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.EventTypes.Command.UpdateEventType;
public class UpdateEventTypeCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}
public class UpdateEventTypeCommandHandler : IRequestHandler<UpdateEventTypeCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateEventTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(UpdateEventTypeCommand request, CancellationToken cancellationToken)
    {
        var eventTypes = await _context.EventTypes.FirstOrDefaultAsync(x => x.Id == request.Id);
        Guard.Against.NotFound(request.Id, eventTypes);
        eventTypes.Name = request.Name;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
