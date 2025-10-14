using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Mappings;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.ProjectContacts.Queries;
using ProjectManagement.Application.Projects.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.ProjectContacts.Queries;

public class GetProjectContactWithPaginationQuery: IRequest<PaginatedList<ProjectContactDto>>
{
    public string? Search { get; set; }
    public int PageNumber { get; init; } = 1;
     public int PageSize { get; init; } = 10;
}
public class GetProjectContactWithPaginationQueryHandler : IRequestHandler<GetProjectContactWithPaginationQuery, PaginatedList<ProjectContactDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProjectContactWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<PaginatedList<ProjectContactDto>> Handle(GetProjectContactWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var projectContactsdto = _context.ProjectContacts
                 .Include(x => x.Projects)
            .Include(x => x.OrganizationContacts).AsQueryable();
        if (!string.IsNullOrEmpty(request.Search))
        {
            projectContactsdto = projectContactsdto
     .Where(x => x.OrganizationContacts!.FirstName.ToLower().Contains(request.Search.ToLower()));
        }
        return await projectContactsdto
                .OrderByDescending(x => x.Created)
                .ProjectTo<ProjectContactDto>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
