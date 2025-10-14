using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.EmailLogs.Queries;
using ProjectManagement.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace ProjectManagement.Application.LDAPSettings.Queries;

public class EmailLogByDateQuery : IRequest<List<EmailLogDto>>
{
    public DateTime? SDate { get; set; }
    public DateTime? EDate { get; set; }
}
public class GetEmailLogByDateQueryHandler : IRequestHandler<EmailLogByDateQuery, List<EmailLogDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmailLogByDateQueryHandler(IApplicationDbContext _Context, IMapper mapper)
    {
        _context = _Context;
        _mapper = mapper;
    }

    public async Task<List<EmailLogDto>> Handle(EmailLogByDateQuery request, CancellationToken cancellationToken)
    {
        if (request.SDate.HasValue && request.SDate != DateTime.MinValue && request.EDate.HasValue && request.EDate != DateTime.MinValue)
        {
            var startDate = request.SDate.HasValue
    ? (request.SDate.Value.Kind == DateTimeKind.Utc
        ? DateTime.SpecifyKind(request.SDate.Value.Date, DateTimeKind.Unspecified) // ใช้ Date เพื่อลบเวลา
        : request.SDate.Value.Date)
    : (DateTime?)null;

            var endDate = request.EDate.HasValue
                ? (request.EDate.Value.Kind == DateTimeKind.Utc
                    ? DateTime.SpecifyKind(request.EDate.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Unspecified) // ใช้ AddDays(1) แล้วลด 1 tick เพื่อได้ 23:59:59
                    : request.EDate.Value.Date.AddDays(1).AddTicks(-1))
                : (DateTime?)null;

            var emailLogs = await _context.EmailLogs
                .Where(x => x.SendDate >= startDate && x.SendDate <= endDate)
                .ToListAsync(cancellationToken);

            //Guard.Against.NotFound(request.SDate, emailLogs);

            // Distinct โดยเลือก EmailLog ที่มี SendDate ล่าสุด
            var distinctEmailLogs = emailLogs
                .GroupBy(x => x.SentTo) // จัดกลุ่มตาม Email
                .Select(g => g.OrderByDescending(x => x.SendDate).First()) // เอาตัวที่ส่งล่าสุด
                .Where(x => x.SendStatus == false) // กรองเฉพาะที่ SendStatus == false
                .ToList();

            var emailLogDtos = _mapper.Map<List<EmailLog>, List<EmailLogDto>>(distinctEmailLogs);
            return emailLogDtos;
            
        }
        else
        {
            throw new ArgumentException("StartDate และ EndDate ต้องไม่เป็นค่าว่าง");
        }
    }

}

