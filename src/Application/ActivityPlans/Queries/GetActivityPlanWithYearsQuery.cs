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

public record GetActivityPlanWithYearsQuery : IRequest<ActivityPlanYearsDto>;

public class GetActivityPlanWithYearsQueryHandler : IRequestHandler<GetActivityPlanWithYearsQuery, ActivityPlanYearsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetActivityPlanWithYearsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ActivityPlanYearsDto> Handle(GetActivityPlanWithYearsQuery request, CancellationToken cancellationToken)
    {
        var dto = new ActivityPlanYearsDto();
        var now = DateTime.Now;
        #region ปีนี้ vs ปีที่แล้ว
        var currentYearStart = new DateTime(now.Year, 1, 1);
        var currentYearEnd = new DateTime(now.Year, 12, 31);

        var lastYearStart = new DateTime(now.Year - 1, 1, 1);
        var lastYearEnd = new DateTime(now.Year - 1, 12, 31);

        var currentActivities = await _context.ActivityPlans
            .Include(x => x.Employees).ThenInclude(e => e!.Departments)
            .Include(x => x.Projects)
            .Include(x => x.Organizations)
            .Include(x => x.EventTypes)
            .Where(x => x.StartDate.Date >= currentYearStart && x.StartDate.Date <= currentYearEnd && x.EventTypes!.EventTypeCode == "001")
            .ToListAsync(cancellationToken);

       var previousActivities = await _context.ActivityPlans
            .Include(x => x.Employees)
            .Include(x => x.Projects)
            .Include(x => x.Organizations)
            .Include(x => x.EventTypes)
            .Where(x => x.StartDate.Date >= lastYearStart && x.StartDate.Date <= lastYearEnd && x.EventTypes!.EventTypeCode == "001")
            .ToListAsync(cancellationToken);
        #endregion

        #region นับจำนวนในแต่ละเดือนของปีปัจจุบัน (แบบ Optimized)
        var currentMonthGroups = currentActivities
            .GroupBy(x => x.StartDate.Month)
            .ToDictionary(g => g.Key, g => g.Count());

        dto.CurrentYears = new YearsDto
        {
            Jan = currentMonthGroups.GetValueOrDefault(1, 0),
            Feb = currentMonthGroups.GetValueOrDefault(2, 0),
            Mar = currentMonthGroups.GetValueOrDefault(3, 0),
            Apr = currentMonthGroups.GetValueOrDefault(4, 0),
            May = currentMonthGroups.GetValueOrDefault(5, 0),
            Jun = currentMonthGroups.GetValueOrDefault(6, 0),
            Jul = currentMonthGroups.GetValueOrDefault(7, 0),
            Aug = currentMonthGroups.GetValueOrDefault(8, 0),
            Sep = currentMonthGroups.GetValueOrDefault(9, 0),
            Oct = currentMonthGroups.GetValueOrDefault(10, 0),
            Nov = currentMonthGroups.GetValueOrDefault(11, 0),
            Dec = currentMonthGroups.GetValueOrDefault(12, 0)
        };
        #endregion

        #region นับจำนวนในแต่ละเดือนของปีที่แล้ว (แบบ Optimized)
        var previousMonthGroups = previousActivities
            .GroupBy(x => x.StartDate.Month)
            .ToDictionary(g => g.Key, g => g.Count());

        dto.OldYears = new YearsDto
        {
            Jan = previousMonthGroups.GetValueOrDefault(1, 0),
            Feb = previousMonthGroups.GetValueOrDefault(2, 0),
            Mar = previousMonthGroups.GetValueOrDefault(3, 0),
            Apr = previousMonthGroups.GetValueOrDefault(4, 0),
            May = previousMonthGroups.GetValueOrDefault(5, 0),
            Jun = previousMonthGroups.GetValueOrDefault(6, 0),
            Jul = previousMonthGroups.GetValueOrDefault(7, 0),
            Aug = previousMonthGroups.GetValueOrDefault(8, 0),
            Sep = previousMonthGroups.GetValueOrDefault(9, 0),
            Oct = previousMonthGroups.GetValueOrDefault(10, 0),
            Nov = previousMonthGroups.GetValueOrDefault(11, 0),
            Dec = previousMonthGroups.GetValueOrDefault(12, 0)
        };
        #endregion
        return dto;


    }
}
