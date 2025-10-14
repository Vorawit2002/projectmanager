using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.EmailMessageSettings.Queries;

public record GetEmailMessageSettingByIdQuery(Guid Id) : IRequest<EmailMessageSettingDto>;

public class GetMailSettingByIdQueryHandler : IRequestHandler<GetEmailMessageSettingByIdQuery, EmailMessageSettingDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetMailSettingByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<EmailMessageSettingDto> Handle(GetEmailMessageSettingByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.EmailMessageSettings
            .FirstOrDefaultAsync(l => l.Id == request.Id);

        Guard.Against.NotFound(request.Id, entity);

        var mailSettingDto = _mapper.Map<EmailMessageSetting, EmailMessageSettingDto>(entity);

        return mailSettingDto;

    }
}
