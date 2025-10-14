using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Projects.Command.UpdateProjectContact;
public class UpdateProjectContactCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid OrganizationContactId { get; set; }
}
public class UpdateProjectContactCommandHandler : IRequestHandler<UpdateProjectContactCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateProjectContactCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(UpdateProjectContactCommand request, CancellationToken cancellationToken)
    {
        var projectContact = await _context.ProjectContacts.FirstOrDefaultAsync(x => x.Id == request.Id);
        Guard.Against.NotFound(request.Id, projectContact);
        projectContact.ProjectId = request.ProjectId;
        projectContact.OrganizationContactId = request.OrganizationContactId;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
