using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;

namespace ProjectManagement.Application.EmailMessageSettings.Queries;
public class GetEmailMessageSettingQuery : IRequest<IEnumerable<EmailMessageSettingDto>>
{
}
public class GetMailSettingQueryHandle : IRequestHandler<GetEmailMessageSettingQuery, IEnumerable<EmailMessageSettingDto>>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public GetMailSettingQueryHandle(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }
    public async Task<IEnumerable<EmailMessageSettingDto>> Handle(GetEmailMessageSettingQuery request, CancellationToken cancellationToken)
    {
        return await _applicationDbContext.EmailMessageSettings
            .ProjectTo<EmailMessageSettingDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}
