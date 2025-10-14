using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Organizations.Commands.DeleteOrganization;
public record DeleteOrganizationCommand(Guid Id) : IRequest<bool>;

public class DeleteOrganizationCommandHandler : IRequestHandler<DeleteOrganizationCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteOrganizationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organizations = await _context.Organizations.FirstOrDefaultAsync(x => x.Id == request.Id);
        Guard.Against.NotFound(request.Id, organizations);
        _context.Organizations.Remove(organizations);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
