using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Mappings;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Employees.Queries;
using ProjectManagement.Application.Organizations.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Departments.Queries;

public class GetDepartmentWithPaginationQuery: IRequest<PaginatedList<DepartmentDto>>
{
     public int PageNumber { get; init; } = 1;
     public int PageSize { get; init; } = 10;
}
public class GetDepartmentWithPaginationQueryHandler : IRequestHandler<GetDepartmentWithPaginationQuery, PaginatedList<DepartmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDepartmentWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<PaginatedList<DepartmentDto>> Handle(GetDepartmentWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return await _context.Departments
                .OrderBy(x => x.Name)
                .ProjectTo<DepartmentDto>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize);

    }
}
