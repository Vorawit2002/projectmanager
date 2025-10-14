using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Mappings;
using ProjectManagement.Application.Common.Models;

namespace ProjectManagement.Application.EmailMessageSettings.Queries;
public class GetEmailMessageSettingWithPaginationQuery : IRequest<PaginatedList<EmailMessageSettingDto>>
{
    public string? search { get; init; } = string.Empty;
    public bool? IsActive { get; init; } = null;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
public class GetMailSettingWithPaginationQueryHandler : IRequestHandler<GetEmailMessageSettingWithPaginationQuery, PaginatedList<EmailMessageSettingDto>>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public GetMailSettingWithPaginationQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }
    public async Task<PaginatedList<EmailMessageSettingDto>> Handle(GetEmailMessageSettingWithPaginationQuery request, CancellationToken cancellationToken)
    {
        string search = request.search?? string.Empty;
        if (request.IsActive == false || request.IsActive == true)
        {
            return await _applicationDbContext.EmailMessageSettings
            .Where(x => (x.SettingCode.Contains(search) || x.SettingName.Contains(search) || x.MailSubject.Contains(search)) && x.IsActive == Convert.ToBoolean(request.IsActive))
           .OrderBy(x => x.SettingCode)
           .ProjectTo<EmailMessageSettingDto>(_mapper.ConfigurationProvider)
           .PaginatedListAsync(request.PageNumber, request.PageSize);
        }
        else
        {
            return await _applicationDbContext.EmailMessageSettings
            .Where(x => x.SettingCode.Contains(search) || x.SettingName.Contains(search) || x.MailSubject.Contains(search))
          .OrderBy(x => x.SettingCode)
          .ProjectTo<EmailMessageSettingDto>(_mapper.ConfigurationProvider)
          .PaginatedListAsync(request.PageNumber, request.PageSize);
        }
        }
}
