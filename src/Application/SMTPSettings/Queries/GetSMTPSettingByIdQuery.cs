using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.SMTPSettings.Queries;
public record GetSMTPSettingByIdQuery(Guid Id) : IRequest<SMTPSettingDto>;
public class GetConfigMailByIdQueryHandler : IRequestHandler<GetSMTPSettingByIdQuery, SMTPSettingDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetConfigMailByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<SMTPSettingDto> Handle(GetSMTPSettingByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.SMTPSettings
            .FirstOrDefaultAsync(l => l.Id == request.Id);

        Guard.Against.NotFound(request.Id, entity);

        var configMailDto = _mapper.Map<SMTPSetting, SMTPSettingDto>(entity);

        return configMailDto;

    }
}
