using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Common.Mappings;

namespace ProjectManagement.Application.EmailScheduleSettings.Queries;
public class GetEmailScheduleSettingQueryWithPagination : IRequest<PaginatedList<EmailScheduleSettingDto>>
{
    public string search { get; set; } = null!;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
public class GetEmailScheduleSettingQueryWithPaginationHandle : IRequestHandler<GetEmailScheduleSettingQueryWithPagination, PaginatedList<EmailScheduleSettingDto>>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public GetEmailScheduleSettingQueryWithPaginationHandle(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }
    public async Task<PaginatedList<EmailScheduleSettingDto>> Handle(GetEmailScheduleSettingQueryWithPagination request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request.search))
        {
            return await _applicationDbContext.EmailScheduleSettings
                .Where(x => x.ScheduleName.Contains(request.search))
            .OrderBy(x => x.Created)
            .ProjectTo<EmailScheduleSettingDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
        }
        else
        {
            return await _applicationDbContext.EmailScheduleSettings
            .OrderBy(x => x.Created)
            .ProjectTo<EmailScheduleSettingDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
        }
    }
}
