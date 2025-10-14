using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Organizations.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.OrganizationContacts.Queries;

public class GetOrganizationContactByOrganizationIdQuery : IRequest<IEnumerable<OrganizationContactDto>>
{
    public Guid OrganizationId { get; set; }
}

public class GetOrganizationContactByOrganizationIdQueryHandler : IRequestHandler<GetOrganizationContactByOrganizationIdQuery, IEnumerable<OrganizationContactDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetOrganizationContactByOrganizationIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<OrganizationContactDto>> Handle(GetOrganizationContactByOrganizationIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.OrganizationContacts.Where(x => x.OrganizationId == request.OrganizationId)
            .ProjectTo<OrganizationContactDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
         
    }
}
