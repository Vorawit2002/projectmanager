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

public record GetEmployeeByUserIdQuery(string UserId) : IRequest<EmployeeDto>;

public class GetEmployeeByUserIdQueryHandler : IRequestHandler<GetEmployeeByUserIdQuery, EmployeeDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeByUserIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<EmployeeDto> Handle(GetEmployeeByUserIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Employees
            .Include(x => x.Departments)
            .FirstOrDefaultAsync(l => l.UserId == request.UserId);
        Guard.Against.NotFound(request.UserId, entity);

        var employeeDto = _mapper.Map<Employee, EmployeeDto>(entity);

        return employeeDto;
    }
}
