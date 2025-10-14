using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Mappings;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Organizations.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.OrganizationContacts.Queries;

public class GetOrganizationContactWithPaginationQuery: IRequest<PaginatedList<OrganizationContactDto>>
{
    public string? Search { get; set; }
    public Guid? OrganizationId { get; set; } 
    public int PageNumber { get; init; } = 1;
     public int PageSize { get; init; } = 10;
}
public class GetOrganizationContactWithPaginationQueryHandler : IRequestHandler<GetOrganizationContactWithPaginationQuery, PaginatedList<OrganizationContactDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetOrganizationContactWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<PaginatedList<OrganizationContactDto>> Handle(GetOrganizationContactWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var QueryOrganizationContact =  _context.OrganizationContacts.AsQueryable();
        if (!string.IsNullOrEmpty(request.Search))
        {
            QueryOrganizationContact = QueryOrganizationContact.Where(x => x.FirstName.ToLower().Contains(request.Search.ToLower()) || (x.LastName != null && x.LastName.ToLower().Contains(request.Search.ToLower())) ||
    (x.Email != null && x.Email.ToLower().Contains(request.Search.ToLower())));
        }
        if(request.OrganizationId != Guid.Empty && request.OrganizationId != null)
        {
            QueryOrganizationContact = QueryOrganizationContact.Where(x => x.OrganizationId == request.OrganizationId);
        }
        return await QueryOrganizationContact
                .OrderBy(x => x.FirstName)
                .ProjectTo<OrganizationContactDto>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize);

    }
}
