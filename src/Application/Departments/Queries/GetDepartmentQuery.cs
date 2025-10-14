using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Departments.Queries;
using ProjectManagement.Application.Employees.Queries;
using ProjectManagement.Application.Organizations.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Departments.Queries;

public record GetDepartmentQuery: IRequest<IEnumerable<DepartmentDto>>;

public class GetDepartmentQueryHandler : IRequestHandler<GetDepartmentQuery, IEnumerable<DepartmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDepartmentQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<DepartmentDto>> Handle(GetDepartmentQuery request, CancellationToken cancellationToken)
    {
        return await _context.Departments
            .ProjectTo<DepartmentDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
         
    }
}
