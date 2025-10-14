using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.ActivityPlans.Queries;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Mappings;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Employees.Queries;
using ProjectManagement.Application.Organizations.Queries;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ProjectManagement.Application.ActivityPlans.Queries;

public class GetActivityPlanWithPaginationQuery : IRequest<PaginatedListForActivity<ActivityPlanDto>>
{
    public List<Guid>? EmployeeId { get; set; }
    public string? search { get; set; }
    public int PageNumber { get; init; } = 1;
     public int PageSize { get; init; } = 10;
    public string SortColumn { get; init; } = "Created"; // เพิ่ม default valu
    public Boolean SortDesc { get; init; }
    public ActivityPlanStatus? ActivityPlanStatus { get; init; }
    public List<Guid>? DepartmentId { get; set; }
    public List<Guid>? EventTypeId { get; set; }
    //public Guid? EventTypeId { get; set; }
    public string? Date{ get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

}
public class GetActivityPlanWithPaginationQueryHandler : IRequestHandler<GetActivityPlanWithPaginationQuery, PaginatedListForActivity<ActivityPlanDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetActivityPlanWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<PaginatedListForActivity<ActivityPlanDto>> Handle(GetActivityPlanWithPaginationQuery request, CancellationToken cancellationToken)
    {

        var QueryActivityPlans = _context.ActivityPlans.Include(x => x.Organizations).Include(x => x.Projects).Include(x => x.PlanNotes)
            .OrderByDescending(x => x.Created).AsQueryable();
        DateTime oneWeekAgo = DateTime.Today.AddDays(-7);
        var today = DateTime.Today;
        #region filter
        if (!string.IsNullOrEmpty(request.search))
        {
            QueryActivityPlans = QueryActivityPlans.Where(x => (x.Objective != null && x.Objective.ToLower().Contains(request.search.ToLower())) || x.Organizations!.Name.ToLower().Contains(request.search.ToLower()) || (x.Projects != null && x.Projects.ProjectCode.ToLower().Contains(request.search.ToLower())));
        }
        if (request.EventTypeId?.Any() == true)
        {
            // ดึง EventTypes ทั้งหมดที่อยู่ในลิสต์
            var eventTypes = await _context.EventTypes
                .Where(x => request.EventTypeId.Contains(x.Id))
                .ToListAsync();

            // Filter เฉพาะที่ EventTypeId อยู่ใน request
            QueryActivityPlans = QueryActivityPlans
                .Where(x => x.EventTypeId.HasValue && request.EventTypeId.Contains(x.EventTypeId.Value));

            // แยกเงื่อนไขตามประเภท EventTypeCode
            if (eventTypes.Any(x => x.EventTypeCode == "002") && string.IsNullOrEmpty(request.Date))
            {
                QueryActivityPlans = QueryActivityPlans
                    .Where(x => x.StartDate.Date <= DateTime.Now.Date && x.EndDate.Date >= DateTime.Now.Date);
            }

            if (eventTypes.Any(x => x.EventTypeCode == "001") && string.IsNullOrEmpty(request.Date))
            {
                QueryActivityPlans = QueryActivityPlans
                    .Where(x => x.StartDate.Date >= oneWeekAgo || (x.StartDate.Date < oneWeekAgo && !x.PlanNotes.Any()));
            }
        }
        if (!string.IsNullOrEmpty(request.Date))
        {
            if (request.Date.Contains("วันนี้"))
            {
                QueryActivityPlans = QueryActivityPlans
                 .Where(x => x.StartDate.Date <= DateTime.Now.Date && x.EndDate.Date >= DateTime.Now.Date);
            }
            else if (request.Date.Contains("สัปดาห์นี้"))
            {
                
                // เริ่มต้นสัปดาห์ (วันจันทร์)
                var diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                var weekStart = today.AddDays(-diff);
                var weekEnd = weekStart.AddDays(6);

                QueryActivityPlans = QueryActivityPlans
                    .Where(x => x.StartDate.Date <= weekEnd && x.EndDate.Date >= weekStart);
            }
            else if (request.Date.Contains("เดือนนี้"))
            {
                var monthStart = new DateTime(today.Year, today.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                QueryActivityPlans = QueryActivityPlans
                    .Where(x => x.StartDate.Date <= monthEnd && x.EndDate.Date >= monthStart);
            }
            else if (request.Date.Contains("ช่วงเวลา") && request.StartDate.HasValue && request.EndDate.HasValue)
            {
                var startDate = request.StartDate.Value.ToLocalTime();
                var endDate = request.EndDate.Value.ToLocalTime();

                QueryActivityPlans = QueryActivityPlans
                    .Where(x => x.StartDate.Date >= startDate.Date && x.EndDate.Date <= endDate.Date);
            }
        }
        if (request.DepartmentId?.Any() == true)
        {
           
            var employeeIds = _context.Employees
       .Where(e => e.DepartmentId !=null && request.DepartmentId.Contains((Guid)e.DepartmentId))
       .Select(e => e.Id)
       .ToList();

            // แล้วกรอง activity ที่อยู่ใน employee กลุ่มนี้
            QueryActivityPlans = QueryActivityPlans.Where(x => employeeIds.Contains(x.EmployeeId));
        }
        if (request.EmployeeId?.Any() == true)
        {
            QueryActivityPlans = QueryActivityPlans.Where(x => request.EmployeeId.Contains(x.EmployeeId));
        }
        if (!string.IsNullOrEmpty(request.SortColumn)) 
        { 
            if (request.SortColumn == "Projects.ProjectName")
            {
                QueryActivityPlans = request.SortDesc
                    ? QueryActivityPlans.OrderByDescending(x => x.Projects != null ? x.Projects.ProjectName : "")
                    : QueryActivityPlans.OrderBy(x => x.Projects != null ? x.Projects.ProjectName : "");
            }
            else if (request.SortColumn == "Employee")
            {
                QueryActivityPlans = request.SortDesc
                    ? QueryActivityPlans.OrderByDescending(x => x.Employees != null ? x.Employees.FirstName : "")
                    : QueryActivityPlans.OrderBy(x => x.Employees != null ? x.Employees.FirstName : "");
            }
            else
            {
                QueryActivityPlans = QueryActivityPlans.OrderByProperty(request.SortColumn, request.SortDesc);
            }
        }
        else
        {
            // Default sorting
            QueryActivityPlans = request.SortDesc
                ? QueryActivityPlans.OrderByDescending(e => e.Created)
                : QueryActivityPlans.OrderBy(e => e.Created);
        }
        if(request.ActivityPlanStatus == ActivityPlanStatus.Summary)
        {
            QueryActivityPlans = QueryActivityPlans.Include(x => x.PlanNotes).Where(x => x.PlanNotes.Any());
        }else if(request.ActivityPlanStatus == ActivityPlanStatus.NotSummary)
        {
            QueryActivityPlans = QueryActivityPlans.Include(x => x.PlanNotes).Where(x => !x.PlanNotes.Any());
        }
        #endregion
        var countitem = await QueryActivityPlans.CountAsync();
        var items = await QueryActivityPlans.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
              .Include(ap => ap.Employees)
        .Include(ap => ap.Projects)
        .Include(ap => ap.Organizations)
         .Include(ap => ap.ActivityPlanAttachments)
        .ThenInclude(at => at.Attachments)
        .Include(ap => ap.EventTypes)
        .Include(ap => ap.PlanNotes) // เพิ่ม Include ตรงนี้
              .Select(activity => new ActivityPlanDto
              {
                  Id = activity.Id,
                  EmployeeId = activity.EmployeeId,
                  Employees = activity.Employees,
                  Objective = activity.Objective,
                  ObjectiveDetail = activity.ObjectiveDetail,
                  detail = activity.detail,
                  ProjectId = activity.ProjectId,
                  Projects = activity.Projects,
                  OrganizationId = activity.OrganizationId,
                  Organizations = activity.Organizations,
                  AllDay = activity.AllDay,
                  StartDate = activity.StartDate,
                  EndDate = activity.EndDate,
                  Location = activity.Location,
                  HaveCost = activity.HaveCost,
                  CostDetail = activity.CostDetail,
                  Cost = activity.Cost,
                   EventTypeId = activity.EventTypeId,
                  EventTypes = activity.EventTypes,
              })
              .ToListAsync(cancellationToken);

        var pagedList = new PaginatedListForActivity<ActivityPlanDto>(items, countitem, request.PageNumber, request.PageSize);
        pagedList.All = await _context.ActivityPlans.CountAsync();
        pagedList.Summary  = await _context.ActivityPlans.Include(x => x.PlanNotes).Where(x => x.PlanNotes.Any()).CountAsync();
        pagedList.NotSummary = await _context.ActivityPlans.Include(x => x.PlanNotes).Where(x => !x.PlanNotes.Any()).CountAsync();

        return pagedList;

    }
}

public static class QueryableExtensions
{
    public static IQueryable<T> OrderByProperty<T>(
        this IQueryable<T> source,
        string propertyName,
        bool isDescending)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            return source;

        // Convert property name to pascal case to match C# naming conventions
        propertyName = char.ToUpper(propertyName[0]) + propertyName[1..].ToLower();

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, propertyName);
        var lambda = Expression.Lambda(property, parameter);

        var orderByMethod = isDescending
            ? nameof(Queryable.OrderByDescending)
            : nameof(Queryable.OrderBy);

        var resultExpression = Expression.Call(
            typeof(Queryable),
            orderByMethod,
            new[] { typeof(T), property.Type },
            source.Expression,
            Expression.Quote(lambda));

        return source.Provider.CreateQuery<T>(resultExpression);
    }
}
