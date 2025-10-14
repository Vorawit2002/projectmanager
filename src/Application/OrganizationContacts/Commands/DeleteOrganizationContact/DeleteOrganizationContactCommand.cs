using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.OrganizationContacts.Commands.DeleteOrganizationContact;
public record DeleteOrganizationContactCommand(Guid Id) : IRequest<bool>;
public class DeleteOrganizationContactCommandHandler : IRequestHandler<DeleteOrganizationContactCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteOrganizationContactCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(DeleteOrganizationContactCommand request, CancellationToken cancellationToken)
    {
        var organizationContacts = await _context.OrganizationContacts.FirstOrDefaultAsync(x => x.Id == request.Id);
        Guard.Against.NotFound(request.Id, organizationContacts);
        _context.OrganizationContacts.Remove(organizationContacts);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
