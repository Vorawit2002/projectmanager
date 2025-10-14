using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Projects.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Projects.Queries;

public record GetProjectByIdQuery(Guid Id) : IRequest<ProjectDto>;

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProjectByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Projects
            .FirstOrDefaultAsync(l => l.Id == request.Id);

        Guard.Against.NotFound(request.Id, entity);
        var projectDto = _mapper.Map<Project, ProjectDto>(entity);

        return projectDto;
    }
}
