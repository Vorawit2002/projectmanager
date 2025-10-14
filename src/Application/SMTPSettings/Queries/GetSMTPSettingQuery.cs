using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;

namespace ProjectManagement.Application.SMTPSettings.Queries;
public class GetSMTPSettingQuery : IRequest<IEnumerable<SMTPSettingDto>>
{
}
public class GetConfigMailQueryHandle : IRequestHandler<GetSMTPSettingQuery, IEnumerable<SMTPSettingDto>>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public GetConfigMailQueryHandle(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }
    public async Task<IEnumerable<SMTPSettingDto>> Handle(GetSMTPSettingQuery request, CancellationToken cancellationToken)
    {
        return await _applicationDbContext.SMTPSettings.Where(l => l.IsActive == true)
            .ProjectTo<SMTPSettingDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}
