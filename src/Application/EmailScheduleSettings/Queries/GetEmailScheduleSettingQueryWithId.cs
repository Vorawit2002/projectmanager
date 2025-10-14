using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Common.Mappings;
using ProjectManagement.Application.SMTPSettings.Queries;
using ProjectManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Application.EmailScheduleSettings.Queries;
public record GetEmailScheduleSettingQueryWithId(Guid Id) : IRequest<EmailScheduleSettingDto>;
public class GetEmailScheduleSettingQueryWithIdHandle : IRequestHandler<GetEmailScheduleSettingQueryWithId, EmailScheduleSettingDto>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public GetEmailScheduleSettingQueryWithIdHandle(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }
    public async Task<EmailScheduleSettingDto> Handle(GetEmailScheduleSettingQueryWithId request, CancellationToken cancellationToken)
    {
        var entity = await _applicationDbContext.EmailScheduleSettings.FirstOrDefaultAsync(x => x.Id == request.Id);

        Guard.Against.NotFound(request.Id, entity);

        var emailScheduleSettingDto = _mapper.Map<EmailScheduleSetting, EmailScheduleSettingDto>(entity);

        return emailScheduleSettingDto;
    }
}
