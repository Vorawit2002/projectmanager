using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Organizations.Commands.CreateOrganization;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.ActivityPlanAttachments.Command.DeleteActivityPlanAttachment;
public record DeleteActivityPlanAttachmentCommand(Guid Id) : IRequest<bool>;

public class DeleteActivityPlanAttachmentCommandHandler : IRequestHandler<DeleteActivityPlanAttachmentCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteActivityPlanAttachmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(DeleteActivityPlanAttachmentCommand request, CancellationToken cancellationToken)
    {
        var activityPlanAttachments = await _context.ActivityPlanAttachments.FirstOrDefaultAsync(x => x.Id == request.Id);
        Guard.Against.NotFound(request.Id, activityPlanAttachments);
         _context.ActivityPlanAttachments.Remove(activityPlanAttachments);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
