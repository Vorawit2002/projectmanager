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

public record GetProjectContactQuery : IRequest<IEnumerable<ProjectContactDto>>;
public class GetProjectContactQueryHandler : IRequestHandler<GetProjectContactQuery, IEnumerable<ProjectContactDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProjectContactQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<ProjectContactDto>> Handle(GetProjectContactQuery request, CancellationToken cancellationToken)
    {
        return await _context.ProjectContacts
            .ProjectTo<ProjectContactDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}
