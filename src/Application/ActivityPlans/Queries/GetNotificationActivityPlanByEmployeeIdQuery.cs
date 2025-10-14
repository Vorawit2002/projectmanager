using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Departments.Queries;
using ProjectManagement.Application.Employees.Queries;
using ProjectManagement.Application.Organizations.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.ActivityPlans.Queries;

public class GetNotificationActivityPlanByEmployeeIdQuery : IRequest<IEnumerable<ActivityPlanNotificationDto>>
{
    public Guid employeeId { get; set; }
}

public class GetNotificationActivityPlanByEmployeeIdQueryHandler : IRequestHandler<GetNotificationActivityPlanByEmployeeIdQuery, IEnumerable<ActivityPlanNotificationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetNotificationActivityPlanByEmployeeIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<ActivityPlanNotificationDto>> Handle(GetNotificationActivityPlanByEmployeeIdQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.Now;
        var today = now.Date;
        var twoDaysFromNow = now.AddDays(2).Date;

        return await _context.ActivityPlans
            .Include(ap => ap.Employees)
            .Include(ap => ap.Projects)
            .Include(ap => ap.Organizations)
            .Include(ap => ap.EventTypes)
            .Include(ap => ap.PlanNotes) // 🔸 Include PlanNotes
            .Where(ap => ap.EmployeeId == request.employeeId && ap.EventTypes!.EventTypeCode == "001")
            .Where(ap => !ap.PlanNotes.Any()) // 🔸 เงื่อนไข: ยังไม่มีการสรุปผล (ไม่มี PlanNote)
            .Where(ap =>
                (ap.StartDate.Date >= today && ap.StartDate.Date <= twoDaysFromNow) // แผนนัดหมายเร็ว ๆ นี้
                || ap.StartDate.Date < today) // แจ้งเตือนการสรุปผล
            .Select(ap => new ActivityPlanNotificationDto
            {
                Id = ap.Id,
                EmployeeId = ap.EmployeeId,
                Employees = ap.Employees,
                Objective = ap.Objective,
                OrganizationId = ap.OrganizationId,
                Organizations = ap.Organizations,
                AllDay = ap.AllDay,
                StartDate = ap.StartDate,
                EndDate = ap.EndDate,
                Title = ap.StartDate.Date < today
                    ? "แจ้งเตือนการสรุปผล"
                    : "แผนนัดหมายเร็ว ๆ นี้"
            })
            .ToListAsync();
    }
}
