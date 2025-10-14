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

public class GetActivityPlanQueryForDashboard : IRequest<DashboardActivityPlanDto>
{
    public string Type { get; set; } = default!; // (ไตรมาส ปีนี้ 5ปีย้อนหลัง)
}

public class GetActivityPlanQueryForDashboardHandler : IRequestHandler<GetActivityPlanQueryForDashboard, DashboardActivityPlanDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetActivityPlanQueryForDashboardHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<DashboardActivityPlanDto> Handle(GetActivityPlanQueryForDashboard request, CancellationToken cancellationToken)
    {
        var dto = new DashboardActivityPlanDto();
        var now = DateTime.Now;

        List<ActivityPlan> currentActivities = new();
        List<ActivityPlan> previousActivities = new();

        if (request.Type == "Quarter")
        {
            #region ไตรมาส
            int currentQuarter = (now.Month - 1) / 3 + 1;

            var quarterStart = new DateTime(now.Year, (currentQuarter - 1) * 3 + 1, 1);
            var quarterEnd = quarterStart.AddMonths(3).AddDays(-1);

            DateTime prevQuarterStart;
            if (currentQuarter == 1)
                prevQuarterStart = new DateTime(now.Year - 1, 10, 1);
            else
                prevQuarterStart = new DateTime(now.Year, (currentQuarter - 2) * 3 + 1, 1);
            var prevQuarterEnd = prevQuarterStart.AddMonths(3).AddDays(-1);

            currentActivities = await _context.ActivityPlans
                .Include(x => x.Employees).ThenInclude(e => e!.Departments)
                .Include(x => x.Projects)
                .Include(x => x.Organizations)
                    .Include(x => x.EventTypes)
                .Where(x => x.StartDate.Date >= quarterStart && x.StartDate.Date <= quarterEnd && x.EventTypes!.EventTypeCode == "001")
                .ToListAsync(cancellationToken);

            previousActivities = await _context.ActivityPlans
                .Include(x => x.Employees)
                .Include(x => x.Projects)
                .Include(x => x.Organizations)
                    .Include(x => x.EventTypes)
                .Where(x => x.StartDate.Date >= prevQuarterStart && x.StartDate.Date <= prevQuarterEnd && x.EventTypes!.EventTypeCode == "001")
                .ToListAsync(cancellationToken);
            #endregion
        }
        else if (request.Type == "Years")
        {
            #region ปีนี้ vs ปีที่แล้ว
            var currentYearStart = new DateTime(now.Year, 1, 1);
            var currentYearEnd = new DateTime(now.Year, 12, 31);

            var lastYearStart = new DateTime(now.Year - 1, 1, 1);
            var lastYearEnd = new DateTime(now.Year - 1, 12, 31);

            currentActivities = await _context.ActivityPlans
                .Include(x => x.Employees).ThenInclude(e => e!.Departments)
                .Include(x => x.Projects)
                .Include(x => x.Organizations)
                    .Include(x => x.EventTypes)
                .Where(x => x.StartDate.Date >= currentYearStart && x.StartDate.Date <= currentYearEnd && x.EventTypes!.EventTypeCode == "001")
                .ToListAsync(cancellationToken);

            previousActivities = await _context.ActivityPlans
                .Include(x => x.Employees)
                .Include(x => x.Projects)
                .Include(x => x.Organizations)
                    .Include(x => x.EventTypes)
                .Where(x => x.StartDate.Date >= lastYearStart && x.StartDate.Date <= lastYearEnd && x.EventTypes!.EventTypeCode == "001")
                .ToListAsync(cancellationToken);
            #endregion
        }
        else if (request.Type == "YearAgo")
        {
            #region 5 ปีย้อนหลัง vs 5 ปีก่อนหน้า

            var current1YearStart = new DateTime(now.Year - 1, 1, 1);
            var current1YearEnd = new DateTime(now.Year - 1, 12, 31);

            var previous1YearStart = new DateTime(now.Year - 2, 1, 1); // ก่อนหน้าอีก 5 ปี
            var previous1YearEnd = new DateTime(now.Year - 2, 12, 31);

            currentActivities = await _context.ActivityPlans
                .Include(x => x.Employees).ThenInclude(e => e!.Departments)
                .Include(x => x.Projects)
                .Include(x => x.Organizations)
                    .Include(x => x.EventTypes)
                .Where(x => x.StartDate.Date >= current1YearStart && x.StartDate.Date <= current1YearEnd && x.EventTypes!.EventTypeCode == "001")
                .ToListAsync(cancellationToken);

            previousActivities = await _context.ActivityPlans
                .Include(x => x.Employees)
                .Include(x => x.Projects)
                .Include(x => x.Organizations)
                    .Include(x => x.EventTypes)
                .Where(x => x.StartDate.Date >= previous1YearStart && x.StartDate.Date <= previous1YearEnd && x.EventTypes!.EventTypeCode == "001")
                .ToListAsync(cancellationToken);

            #endregion
        }
      
        // เปอร์เซ็นต์เปลี่ยนแปลง
        int currentCount = currentActivities.Count;
        int previousCount = previousActivities.Count;

        dto.Percent = previousCount == 0 ? 100 : ((decimal)(currentCount - previousCount) / previousCount) * 100;

        // วัตถุประสงค์
        dto.Objectives = currentActivities
            .Select(x => string.IsNullOrWhiteSpace(x.Objective) ? x.ObjectiveDetail : x.Objective)
            .GroupBy(obj => obj)
            .Select(g => new ObjectiveCountDto
            {
                Name = g.Key ?? "ไม่ระบุ",
                Count = g.Count()
            })
            .ToList();
        dto.Cost = currentActivities
     .Where(x => x.Cost.HasValue)
     .Sum(x => x.Cost!.Value);
        // แผนกต่าง ๆ
        dto.CountSale = currentActivities
            .Count(x => x.Employees?.Departments?.Name?.Contains("Sale", StringComparison.OrdinalIgnoreCase) == true);

        dto.CountDevelopment = currentActivities
            .Count(x =>
                x.Employees?.Departments?.Name != null &&
                (
                    x.Employees.Departments.Name.Contains("dev", StringComparison.OrdinalIgnoreCase) ||
                    x.Employees.Departments.Name.Contains("พัฒนา", StringComparison.OrdinalIgnoreCase)
                )
            );

        dto.CountProduct = currentActivities
            .Count(x => x.Employees?.Departments?.Name?.Contains("Product", StringComparison.OrdinalIgnoreCase) == true);

        dto.CountSystemService = currentActivities
            .Count(x => x.Employees?.Departments?.Name?.Contains("System", StringComparison.OrdinalIgnoreCase) == true);

        return dto;
    
}
}
