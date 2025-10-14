using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Mappings;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Organizations.Queries;
using ProjectManagement.Application.ProjectContacts.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Organizations.Queries;

public class GetOrganizationWithPaginationQuery: IRequest<PaginatedList<OrganizationDto>>
{
    public string? Search { get; set; }
    public int PageNumber { get; init; } = 1;
     public int PageSize { get; init; } = 10;
}
public class GetOrganizationWithPaginationQueryHandler : IRequestHandler<GetOrganizationWithPaginationQuery, PaginatedList<OrganizationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetOrganizationWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<PaginatedList<OrganizationDto>> Handle(GetOrganizationWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var organizationdto = _context.Organizations.AsQueryable();
        if (!string.IsNullOrEmpty(request.Search))
        {
            organizationdto = organizationdto
            .Where(x => x.Name.ToLower().Contains(request.Search.ToLower()));
        }
        return await organizationdto
                .OrderByDescending(x => x.Created)
                .ProjectTo<OrganizationDto>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
