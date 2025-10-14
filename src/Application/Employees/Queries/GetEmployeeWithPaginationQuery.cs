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

namespace ProjectManagement.Application.Employees.Queries;

public class GetEmployeeWithPaginationQuery: IRequest<PaginatedList<EmployeeDto>>
{
     public int PageNumber { get; init; } = 1;
     public int PageSize { get; init; } = 10;
}
public class GetEmployeeWithPaginationQueryHandler : IRequestHandler<GetEmployeeWithPaginationQuery, PaginatedList<EmployeeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<PaginatedList<EmployeeDto>> Handle(GetEmployeeWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return await _context.Employees
                .OrderBy(x => x.FirstName)
                .Include(x => x.Departments)
                .ProjectTo<EmployeeDto>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize);

    }
}
