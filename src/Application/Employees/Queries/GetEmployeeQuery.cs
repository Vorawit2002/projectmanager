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

public record GetEmployeeQuery : IRequest<IEnumerable<EmployeeDto>>;
public class GetEmployeeQueryHandler : IRequestHandler<GetEmployeeQuery, IEnumerable<EmployeeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<EmployeeDto>> Handle(GetEmployeeQuery request, CancellationToken cancellationToken)
    {
        return await _context.Employees
            .ProjectTo<EmployeeDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
         
    }
}
