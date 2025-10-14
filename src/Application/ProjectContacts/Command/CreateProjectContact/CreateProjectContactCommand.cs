using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Projects.Command.CreateProjectContact;
public class CreateProjectContactCommand : IRequest<bool>
{
    public Guid ProjectId { get; set; }
    public Guid OrganizationContactId { get; set; }
}
public class CreateProjectContactCommandHandler : IRequestHandler<CreateProjectContactCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public CreateProjectContactCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(CreateProjectContactCommand request, CancellationToken cancellationToken)
    {
        var projectContact = new ProjectContact();
        projectContact.ProjectId = request.ProjectId;
        projectContact.OrganizationContactId = request.OrganizationContactId;

        await _context.ProjectContacts.AddAsync(projectContact);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
