using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.OrganizationContacts.Commands.CreateOrganizationContact;
public class CreateOrganizationContactJustNameCommand : IRequest<Guid>
{
    public Guid OrganizationId { get; set; }
    public string FirstName { get; set; } = default!;
}
public class CreateOrganizationContactJustNameCommandHandler : IRequestHandler<CreateOrganizationContactJustNameCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateOrganizationContactJustNameCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Guid> Handle(CreateOrganizationContactJustNameCommand request, CancellationToken cancellationToken)
    {
        var organizationContacts = new OrganizationContact();
        organizationContacts.OrganizationId = request.OrganizationId;
        organizationContacts.FirstName = request.FirstName;
        await _context.OrganizationContacts.AddAsync(organizationContacts);
        await _context.SaveChangesAsync(cancellationToken);
        return organizationContacts.Id;
    }
}
