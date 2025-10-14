using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Organizations.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.OrganizationContacts.Queries;

public record GetOrganizationContactQuery : IRequest<IEnumerable<OrganizationContactDto>>;

public class GetOrganizationContactQueryHandler : IRequestHandler<GetOrganizationContactQuery, IEnumerable<OrganizationContactDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetOrganizationContactQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<OrganizationContactDto>> Handle(GetOrganizationContactQuery request, CancellationToken cancellationToken)
    {
        return await _context.OrganizationContacts
            .ProjectTo<OrganizationContactDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
         
    }
}
