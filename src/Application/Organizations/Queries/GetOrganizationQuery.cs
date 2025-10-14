using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Organizations.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Organizations.Queries;

public record GetOrganizationQuery : IRequest<IEnumerable<OrganizationDto>>;

public class GetOrganizationQueryHandler : IRequestHandler<GetOrganizationQuery, IEnumerable<OrganizationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetOrganizationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<OrganizationDto>> Handle(GetOrganizationQuery request, CancellationToken cancellationToken)
    {
        return await _context.Organizations
            .ProjectTo<OrganizationDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
         
    }
}
