using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Organizations.Commands.CreateOrganization;
public class CreateOrganizationJustNameCommand : IRequest<Guid>
{
    public string Name { get; set; } = default!;
}
public class CreateOrganizationJustNameCommandHandler : IRequestHandler<CreateOrganizationJustNameCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateOrganizationJustNameCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Guid> Handle(CreateOrganizationJustNameCommand request, CancellationToken cancellationToken)
    {
        var organizations = new Organization();
        organizations.Name = request.Name;
        await _context.Organizations.AddAsync(organizations);
        await _context.SaveChangesAsync(cancellationToken);
        return organizations.Id;
    }
}
