using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Organizations.Commands.CreateOrganization;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.ActivityPlanContacts.Command.DeleteActivityPlanContact;
public record DeleteActivityPlanContactCommand(Guid Id) : IRequest<bool>;

public class DeleteActivityPlanContactCommandHandler : IRequestHandler<DeleteActivityPlanContactCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteActivityPlanContactCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(DeleteActivityPlanContactCommand request, CancellationToken cancellationToken)
    {
        var activityPlanContacts = await _context.ActivityPlanContacts.FirstOrDefaultAsync(x => x.Id == request.Id);
        Guard.Against.NotFound(request.Id, activityPlanContacts);
         _context.ActivityPlanContacts.Remove(activityPlanContacts);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
