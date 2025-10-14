using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Common.Mappings;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.EmailLogs.Queries;
public class GetEmailLogWithPaginationByEmailQuery : IRequest<PaginatedList<EmailLogDto>>
{
    public string Email { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
public class GetEmailLogWithPaginationByEmailQueryHandle : IRequestHandler<GetEmailLogWithPaginationByEmailQuery, PaginatedList<EmailLogDto>>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public GetEmailLogWithPaginationByEmailQueryHandle(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }
    public async Task<PaginatedList<EmailLogDto>> Handle(GetEmailLogWithPaginationByEmailQuery request, CancellationToken cancellationToken)
    {
        // Set default values if StartDate or EndDate is null or set to minimum date
        var startDate = request.StartDate == DateTime.MinValue ? DateTime.MinValue.ToLocalTime() : request.StartDate.ToLocalTime();
        var endDate = request.EndDate == DateTime.MinValue ? DateTime.MaxValue.ToLocalTime() : request.EndDate.ToLocalTime();

        IQueryable<EmailLog> query = _applicationDbContext.EmailLogs;

        // Apply email filter if provided
        if (!string.IsNullOrEmpty(request.Email))
        {
            query = query.Where(x => x.SentTo.Contains(request.Email));
        }

        // Apply date range filter if specified
        query = query.Where(x => x.Created >= startDate && x.Created <= endDate);

        return await query
            .OrderBy(x => x.Created)
            .ProjectTo<EmailLogDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
