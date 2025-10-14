using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Organizations.Commands.CreateOrganization;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.ActivityPlanContacts.Command.UpdateActivityPlanContact;
public class UpdateActivityPlanContactCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid ActivityPlanId { get; set; }
    public Guid OrganizationContactId { get; set; }

}
public class UpdateActivityPlanContactCommandHandler : IRequestHandler<UpdateActivityPlanContactCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateActivityPlanContactCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(UpdateActivityPlanContactCommand request, CancellationToken cancellationToken)
    {
        var activityPlanContacts = await _context.ActivityPlanContacts.FirstOrDefaultAsync(x => x.Id == request.Id);
        Guard.Against.NotFound(request.Id, activityPlanContacts);
        activityPlanContacts.ActivityPlanId = request.ActivityPlanId;
        activityPlanContacts.OrganizationContactId = request.OrganizationContactId;

        //await _context.ActivityPlanContacts.AddAsync(activityPlanContacts);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
