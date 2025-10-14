using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Organizations.Commands.CreateOrganization;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.ActivityPlanContacts.Command.CreateActivityPlanContact;
public class CreateActivityPlanContactCommand : IRequest<bool>
{
    public Guid ActivityPlanId { get; set; }
    public Guid OrganizationContactId { get; set; }
}
public class CreateActivityPlanContactCommandHandler : IRequestHandler<CreateActivityPlanContactCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public CreateActivityPlanContactCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(CreateActivityPlanContactCommand request, CancellationToken cancellationToken)
    {
        var activityPlanContacts = new ActivityPlanContact();
        activityPlanContacts.ActivityPlanId = request.ActivityPlanId;
        activityPlanContacts.OrganizationContactId = request.OrganizationContactId;

        await _context.ActivityPlanContacts.AddAsync(activityPlanContacts);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
