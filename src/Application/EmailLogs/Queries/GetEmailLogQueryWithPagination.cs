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
public class GetEmailLogQueryWithPagination : IRequest<PaginatedList<EmailLogDto>>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string search { get; set; } = null!;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
public class GetEmailLogQueryWithPaginationHandle : IRequestHandler<GetEmailLogQueryWithPagination, PaginatedList<EmailLogDto>>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public GetEmailLogQueryWithPaginationHandle(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }
    public async Task<PaginatedList<EmailLogDto>> Handle(GetEmailLogQueryWithPagination request, CancellationToken cancellationToken)
    {
      
        IQueryable<EmailLog> query = _applicationDbContext.EmailLogs;

        // Apply email filter if provided
        if (!string.IsNullOrEmpty(request.search))
        {
            query = query.Where(x => x.Subject.Contains(request.search));
        }
        if (request.StartDate.HasValue && request.StartDate != DateTime.MinValue && request.EndDate.HasValue && request.EndDate != DateTime.MinValue)
        {
            var startDate = request.StartDate.HasValue
        ? (request.StartDate.Value.Kind == DateTimeKind.Utc
            ? DateTime.SpecifyKind(request.StartDate.Value.Date, DateTimeKind.Unspecified) // ใช้ Date เพื่อลบเวลา
            : request.StartDate.Value.Date)
        : (DateTime?)null;

            var endDate = request.EndDate.HasValue
                ? (request.EndDate.Value.Kind == DateTimeKind.Utc
                    ? DateTime.SpecifyKind(request.EndDate.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Unspecified) // ใช้ AddDays(1) แล้วลด 1 tick เพื่อได้ 23:59:59
                    : request.EndDate.Value.Date.AddDays(1).AddTicks(-1))
                : (DateTime?)null;
            query = query.Where(x => x.Created >= startDate && x.Created <= endDate);
        }
        // Apply date range filter if specified
        //query = query.Where(x => x.Created >= startDate && x.Created <= endDate);

        return await query
            .OrderByDescending(x => x.Created)
            .ProjectTo<EmailLogDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
