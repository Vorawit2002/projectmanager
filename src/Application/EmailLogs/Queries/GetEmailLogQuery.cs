using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.EmailLogs.Queries;

namespace ProjectManagement.Application.EmailMessageSettings.Queries;
public class EmailLogQuery : IRequest<IEnumerable<EmailLogDto>>
{
}
public class EmailLogQueryHandle : IRequestHandler<EmailLogQuery, IEnumerable<EmailLogDto>>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public EmailLogQueryHandle(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }
    public async Task<IEnumerable<EmailLogDto>> Handle(EmailLogQuery request, CancellationToken cancellationToken)
    {
        return await _applicationDbContext.EmailMessageSettings
            .ProjectTo<EmailLogDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}
