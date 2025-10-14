using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;

namespace ProjectManagement.Application.SMTPSettings.Command.DeleteSMTPSetting;
public record DeleteSMTPSettingCommand(Guid Id) : IRequest<bool>;
public class DeleteConfigMailCommandHandler : IRequestHandler<DeleteSMTPSettingCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteConfigMailCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(DeleteSMTPSettingCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.SMTPSettings
            .Where(l => l.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        _context.SMTPSettings.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
