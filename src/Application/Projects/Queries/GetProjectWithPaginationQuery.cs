using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Mappings;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Projects.Queries;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Projects.Queries;

public class GetProjectWithPaginationQuery: IRequest<PaginatedList<ProjectDto>>
{
    public string? Search { get; set; }
    public int? ProjectType { get; set; } = -1;
    public int PageNumber { get; init; } = 1;
     public int PageSize { get; init; } = 10;
}
public class GetProjectWithPaginationQueryHandler : IRequestHandler<GetProjectWithPaginationQuery, PaginatedList<ProjectDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUser _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IDataFilterService _dataFilterService;

    public GetProjectWithPaginationQueryHandler(
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
    
    public async Task<PaginatedList<ProjectDto>> Handle(GetProjectWithPaginationQuery request, CancellationToken cancellationToken)
    {
        // Apply role-based filtering first
        var userId = _currentUser.Id ?? throw new UnauthorizedAccessException();
        var roles = (await _identityService.GetUserRolesAsync(userId)).ToArray();
        
        var projectdto = _context.Projects.AsQueryable();
        
        // Apply role-based filtering
        projectdto = await _dataFilterService.ApplyRoleBasedFilterAsync(projectdto, userId, roles);
        if (!string.IsNullOrEmpty(request.Search))
        {
            projectdto = projectdto.Where(x => x.ProjectCode.ToLower().Contains(request.Search.ToLower()) || x.ProjectName.ToLower().Contains(request.Search.ToLower()) || (x.ContractNumber != null && x.ContractNumber.ToLower().Contains(request.Search)));
        }
        if(request.ProjectType != -1 && request.ProjectType != null)
        {
            var targetProjectType = (ProjectType)request.ProjectType;
            projectdto = projectdto.Where(x => x.ProjectType != null && x.ProjectType == targetProjectType);
        }
        return await projectdto
                .OrderBy(x => x.ProjectName)
                .ProjectTo<ProjectDto>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize);

    }
}
