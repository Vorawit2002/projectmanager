using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Mappings;
using ProjectManagement.Application.Common.Models;

namespace ProjectManagement.Application.SMTPSettings.Queries;
public class GetSMTPSettingWithPaginationQuery : IRequest<PaginatedList<SMTPSettingDto>>
{
    public string? search { get; set; }
    public bool IsActive { get; set; } = true;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
public class GetConfigMailWithPaginationQueryHandler : IRequestHandler<GetSMTPSettingWithPaginationQuery, PaginatedList<SMTPSettingDto>>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public GetConfigMailWithPaginationQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }
    public async Task<PaginatedList<SMTPSettingDto>> Handle(GetSMTPSettingWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var QuerySMTPSettings = _applicationDbContext.SMTPSettings.AsQueryable();
        if (!string.IsNullOrEmpty(request.search))
        {
            QuerySMTPSettings= QuerySMTPSettings
                .Where (x=>x.SMTPServer.Contains(request.search));
        }
        QuerySMTPSettings= QuerySMTPSettings
            .Where(x=>x.IsActive==request.IsActive);

        return await QuerySMTPSettings
           .OrderBy(x => x.Id)
           .ProjectTo<SMTPSettingDto>(_mapper.ConfigurationProvider)
           .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
