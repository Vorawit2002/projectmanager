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

public record GetDepartmentByIdQuery(Guid Id) : IRequest<DepartmentDto>;

public class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, DepartmentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDepartmentByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<DepartmentDto> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Departments
            .FirstOrDefaultAsync(l => l.Id == request.Id);

        Guard.Against.NotFound(request.Id, entity);

        var departmentDto = _mapper.Map<Department, DepartmentDto>(entity);

        return departmentDto;
    }
}
