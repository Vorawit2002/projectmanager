using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;

namespace ProjectManagement.Application.CheckInCheckOuts.Queries;
public record GetCheckInCheckOutQuery : IRequest<IEnumerable<CheckInCheckOutDto>>;

public class GetCheckInCheckOutQueryHandler : IRequestHandler<GetCheckInCheckOutQuery, IEnumerable<CheckInCheckOutDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCheckInCheckOutQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<CheckInCheckOutDto>> Handle(GetCheckInCheckOutQuery request, CancellationToken cancellationToken)
    {
        return await _context.CheckInCheckOuts
            .Include(x => x.Projects)
            .Include(x => x.Organizations)
            .Include(x => x.Employees)
            .OrderByDescending(x => x.Created)
            .ProjectTo<CheckInCheckOutDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

    }
}
