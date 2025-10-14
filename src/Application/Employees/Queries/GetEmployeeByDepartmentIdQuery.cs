using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Employees.Queries;
using ProjectManagement.Application.Organizations.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Employees.Queries;

public class GetEmployeeByDepartmentIdQuery : IRequest<IEnumerable<EmployeeDto>>
{
    public Guid DepartmentId { get; set; }
}
public class GetEmployeeByDepartmentIdQueryHandler : IRequestHandler<GetEmployeeByDepartmentIdQuery, IEnumerable<EmployeeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeByDepartmentIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<EmployeeDto>> Handle(GetEmployeeByDepartmentIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Employees.Where(x => x.DepartmentId == request.DepartmentId)
            .ProjectTo<EmployeeDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
         
    }
}
