using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;

namespace ProjectManagement.Application.EmailLogs.Commands.DeleteEmailLog;
public record DeleteEmailLogCommand(Guid Id) : IRequest<bool>;
public class DeleteEmailLogCommandHandler : IRequestHandler<DeleteEmailLogCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ISender _sender;

    public DeleteEmailLogCommandHandler(IApplicationDbContext context, ISender sender)
    {
        _context = context;
        _sender = sender;
    }
    public async Task<bool> Handle(DeleteEmailLogCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.EmailLogs
           .Where(l => l.Id == request.Id)
           .SingleOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        _context.EmailLogs.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
