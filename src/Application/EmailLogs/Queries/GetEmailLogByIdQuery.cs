using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.EmailLogs.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.EmailMessageSettings.Queries;

public record EmailLogByIdQuery(Guid Id) : IRequest<EmailLogDto>;

public class EmailLogByIdQueryHandler : IRequestHandler<EmailLogByIdQuery, EmailLogDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public EmailLogByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<EmailLogDto> Handle(EmailLogByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.EmailLogs
            .FirstOrDefaultAsync(l => l.Id == request.Id);

        Guard.Against.NotFound(request.Id, entity);

        var mailLogDto = _mapper.Map<EmailLog, EmailLogDto>(entity);

        return mailLogDto;

    }
}
