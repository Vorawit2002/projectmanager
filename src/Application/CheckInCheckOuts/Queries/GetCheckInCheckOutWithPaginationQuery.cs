using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Mappings;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ProjectManagement.Application.CheckInCheckOuts.Queries;

public class GetCheckInCheckOutWithPaginationQuery : IRequest<PaginatedList<CheckInCheckOutDto>>
{
    public List<Guid>? EmployeeId { get; init; }
    public List<Guid>? DepartmentId { get; init; }
    public DateTime? DateCheck { get; init; }
    public int PageNumber { get; init; } = 1;
     public int PageSize { get; init; } = 10;
    public string? Date { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
public class GetCheckInCheckOutWithPaginationQueryHandler : IRequestHandler<GetCheckInCheckOutWithPaginationQuery, PaginatedList<CheckInCheckOutDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMinIOService _minIOService;
    public GetCheckInCheckOutWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper, IMinIOService minIOService)
    {
        _context = context;
        _mapper = mapper;
        _minIOService = minIOService;
    }
    public async Task<PaginatedList<CheckInCheckOutDto>> Handle(GetCheckInCheckOutWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.Today;
        var checkInCheckOut = _context.CheckInCheckOuts
                  .Include(x => x.Projects)
                  .Include(x => x.Organizations)
                  .Include(x => x.CheckInCheckOutAttachments)
            .ThenInclude(x => x.Attachments)
                 .Include(x => x.Employees)
                 .OrderByDescending(x => x.Created).AsQueryable();
        if (request.DepartmentId?.Any() == true)
        {
            var employeeIds = _context.Employees
            .Where(e => e.DepartmentId != null && request.DepartmentId.Contains((Guid)e.DepartmentId))
            .Select(e => e.Id)
            .ToList();
            checkInCheckOut = checkInCheckOut.Where(x => employeeIds.Contains(x.EmployeeId));
        }
        if (request.EmployeeId?.Any() == true) 
        {
            checkInCheckOut = checkInCheckOut.Where(x => request.EmployeeId.Contains(x.EmployeeId));
        }
        if (request.DateCheck != null)
        {
            checkInCheckOut = checkInCheckOut.Where(x => x.CheckIn.Date == request.DateCheck.Value.Date.ToLocalTime());
        }
        if (!string.IsNullOrEmpty(request.Date))
        {
            if (request.Date.Contains("วันนี้"))
            {
                checkInCheckOut = checkInCheckOut
                .Where(x => x.CheckIn.Date == DateTime.Now.Date || (x.CheckOut.HasValue && x.CheckOut.Value.Date == DateTime.Now.Date));
            }
            else if (request.Date.Contains("สัปดาห์นี้"))
            {
                // เริ่มต้นสัปดาห์ (วันจันทร์)
                var diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                var weekStart = today.AddDays(-diff);
                var weekEnd = weekStart.AddDays(6);

                checkInCheckOut = checkInCheckOut
                    .Where(x => x.CheckIn.Date <= weekEnd && x.CheckIn.Date >= weekStart);
            }
            else if (request.Date.Contains("เดือนนี้"))
            {
                var monthStart = new DateTime(today.Year, today.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                checkInCheckOut = checkInCheckOut
                    .Where(x => x.CheckIn.Date <= monthEnd && x.CheckIn.Date >= monthStart);
            }
            else if (request.Date.Contains("ช่วงเวลา") && request.StartDate.HasValue && request.EndDate.HasValue)
            {
                var startDate = request.StartDate.Value.ToLocalTime();
                var endDate = request.EndDate.Value.ToLocalTime();

                checkInCheckOut = checkInCheckOut
                    .Where(x => x.CheckIn.Date >= startDate.Date && x.CheckIn.Date <= endDate.Date);
            }
        }
        var result = await checkInCheckOut
         .ProjectTo<CheckInCheckOutDto>(_mapper.ConfigurationProvider)
         .PaginatedListAsync(request.PageNumber, request.PageSize);

        // โหลด base64 image ทีละรายการ แล้วใส่เข้า DTO
        foreach (var item in result.Items)
        {
            foreach (var attachment in item.CheckInCheckOutAttachments)
            {
                byte[] byteArray = await _minIOService.DownloadToByteArray(attachment.Attachments.BucketOriginalName, attachment.Attachments.BucketOriginalPath);
                if (attachment.Type == Domain.Enums.CheckInCheckOutType.CheckIn)
                {
                    item.CheckInImage = Convert.ToBase64String(byteArray);
                }
                else
                {
                    item.CheckOutImage = Convert.ToBase64String(byteArray);
                }
            }
        }
        return result;
    }
}
