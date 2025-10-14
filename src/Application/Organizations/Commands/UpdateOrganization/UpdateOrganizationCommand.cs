using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Organizations.Commands.UpdateOrganization;
public class UpdateOrganizationCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public TypeOrganization? TypeOrganization { get; set; }
    public string? Address { get; set; }
    public string? Coordinates { get; set; }
    public string? WebSite { get; set; } 
    public string? Phone { get; set; } 
    public string? Fax { get; set; }
    public string? ShortName { get; set; }


}
public class UpdateOrganizationCommandHandler : IRequestHandler<UpdateOrganizationCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateOrganizationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organizations = await _context.Organizations.FirstOrDefaultAsync(x => x.Id == request.Id);
        Guard.Against.NotFound(request.Id, organizations);
        organizations.Name = request.Name;
        organizations.TypeOrganization = request.TypeOrganization;
        organizations.Address = request.Address;
        organizations.Coordinates = request.Coordinates;
        organizations.WebSite = request.WebSite;
        organizations.Phone = request.Phone;
        organizations.Fax = request.Fax;
        organizations.ShortName = request.ShortName;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
