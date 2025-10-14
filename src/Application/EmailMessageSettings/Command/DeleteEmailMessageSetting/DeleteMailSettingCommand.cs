using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;

namespace ProjectManagement.Application.EmailMessageSettings.Command.DeleteEmailMessageSetting;

public record DeleteEmailMessageSettingCommand(Guid Id) : IRequest<bool>;
public class DeleteEmailMessageSettingCommandHandler : IRequestHandler<DeleteEmailMessageSettingCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteEmailMessageSettingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(DeleteEmailMessageSettingCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.EmailMessageSettings
            .Where(l => l.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        _context.EmailMessageSettings.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
