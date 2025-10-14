using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Organizations.Commands.CreateOrganization;
public class CreateOrganizationCommand : IRequest<Guid>
{
    public string Name { get; set; } = default!;
    public TypeOrganization? TypeOrganization { get; set; }
    public string? Address { get; set; }
    public string? Coordinates { get; set; }
    public string? WebSite { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? ShortName { get; set; }
}
public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateOrganizationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Guid> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organizations = new Organization();
        organizations.Name = request.Name;
        organizations.TypeOrganization = request.TypeOrganization;
        organizations.Address = request.Address;
        organizations.Coordinates = request.Coordinates;
        organizations.WebSite = request.WebSite;
        organizations.Phone = request.Phone;
        organizations.Fax = request.Fax;
        organizations.ShortName = request.ShortName;
        await _context.Organizations.AddAsync(organizations);
        await _context.SaveChangesAsync(cancellationToken);
        return organizations.Id;
    }
}
