using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Employees.Queries;
using ProjectManagement.Application.Organizations.Queries;
using ProjectManagement.Application.Projects.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Projects.Queries;

public record GetProjectQuery : IRequest<IEnumerable<ProjectDto>>;
public class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, IEnumerable<ProjectDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProjectQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<ProjectDto>> Handle(GetProjectQuery request, CancellationToken cancellationToken)
    {
        return await _context.Projects
            .ProjectTo<ProjectDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}
