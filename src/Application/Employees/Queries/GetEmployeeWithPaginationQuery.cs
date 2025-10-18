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
    private readonly IUser _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IDataFilterService _dataFilterService;

    public GetEmployeeWithPaginationQueryHandler(
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
    
    public async Task<PaginatedList<EmployeeDto>> Handle(GetEmployeeWithPaginationQuery request, CancellationToken cancellationToken)
    {
        // Apply role-based filtering first
        var userId = _currentUser.Id ?? throw new UnauthorizedAccessException();
        var roles = (await _identityService.GetUserRolesAsync(userId)).ToArray();
        
        var query = _context.Employees
            .Include(x => x.Departments)
            .Include(x => x.User) // Include ApplicationUser to get email and imageProfile
            .AsQueryable();
        
        // Apply role-based filtering
        query = await _dataFilterService.ApplyRoleBasedFilterAsync(query, userId, roles);
        
        var employees = await query
            .OrderBy(x => x.FirstName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
        
        var totalCount = await query.CountAsync(cancellationToken);
        
        // Map to DTO with priority: ApplicationUser > Employee
        var dtos = employees.Select(e => new EmployeeDto
        {
            Id = e.Id,
            UserId = e.UserId,
            TitleName = e.TitleName,
            FirstName = e.FirstName ?? e.User?.FirstName,
            LastName = e.LastName ?? e.User?.LastName,
            Email = e.User?.Email ?? e.Email, // Priority: ApplicationUser.Email > Employee.Email
            Position = e.Position,
            Phone = e.Phone,
            ImageProfile = e.User?.ImageProfile ?? e.ImageProfile, // Priority: ApplicationUser.ImageProfile > Employee.ImageProfile
            isActive = e.isActive,
            DepartmentId = e.DepartmentId,
            Departments = e.Departments,
            Created = e.Created,
            CreatedBy = e.CreatedBy,
            LastModified = e.LastModified,
            LastModifiedBy = e.LastModifiedBy,
            Roles = e.Roles,
            Group = e.Group
        }).ToList();
        
        return new PaginatedList<EmployeeDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}
