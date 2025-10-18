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
    private readonly IUser _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IDataFilterService _dataFilterService;

    public GetEmployeeQueryHandler(
        IApplicationDbContext context, 
        IMapper mapper,
        IUser currentUser,
        IIdentityService identityService,
        IDataFilterService dataFilterService)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
        _identityService = identityService;
        _dataFilterService = dataFilterService;
    }
    
    public async Task<IEnumerable<EmployeeDto>> Handle(GetEmployeeQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id ?? throw new UnauthorizedAccessException();
        var roles = (await _identityService.GetUserRolesAsync(userId)).ToArray();
        
        var query = _context.Employees.AsQueryable();
        
        // Apply role-based filtering
        query = await _dataFilterService.ApplyRoleBasedFilterAsync(query, userId, roles);
        
        return await query
            .ProjectTo<EmployeeDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
