using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.ProjectContacts.Queries;
using ProjectManagement.Application.Projects.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.ProjectContacts.Queries;

public record GetProjectContactByIdQuery(Guid Id) : IRequest<ProjectContactDto>;

public class GetProjectContactByIdQueryHandler : IRequestHandler<GetProjectContactByIdQuery, ProjectContactDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProjectContactByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ProjectContactDto> Handle(GetProjectContactByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.ProjectContacts
            .Include(x => x.Projects)
            .Include(x => x.OrganizationContacts)
            .FirstOrDefaultAsync(l => l.Id == request.Id);

        Guard.Against.NotFound(request.Id, entity);
        var projectContactDto = _mapper.Map<ProjectContact, ProjectContactDto>(entity);

        return projectContactDto;
    }
}
