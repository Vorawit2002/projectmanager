using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.ActivityPlans.Queries;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Mappings;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Employees.Queries;
using ProjectManagement.Application.Organizations.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.ActivityPlans.Queries;

public class GetActivityPlanWithPlanNoteWithPaginationQuery : IRequest<PaginatedList<ActivityPlanExcelDto>>
{
    public List<Guid>? EmployeeId { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? search { get; set; }
    public int PageNumber { get; init; } = 1;
     public int PageSize { get; init; } = 10;
    public string? Years { get; set; }
    public int? Month { get; set; } // เพิ่มฟิลด์สำหรับเดือน
}
public class GetActivityPlanWithPlanNoteWithPaginationQueryHandler : IRequestHandler<GetActivityPlanWithPlanNoteWithPaginationQuery, PaginatedList<ActivityPlanExcelDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetActivityPlanWithPlanNoteWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<PaginatedList<ActivityPlanExcelDto>> Handle(GetActivityPlanWithPlanNoteWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var QueryActivityPlans = _context.ActivityPlans.Include(ap => ap.EventTypes).Where(ap => _context.PlanNotes.Any(pn => pn.ActivityPlanId == ap.Id) && ap.EventTypes!.EventTypeCode == "001").AsQueryable();
        #region filter
        if (!string.IsNullOrEmpty(request.search))
        {
            QueryActivityPlans = QueryActivityPlans.Where(x => (x.Objective != null && x.Objective.Contains(request.search)) || x.Organizations!.Name.Contains(request.search) || (x.Projects != null && x.Projects.ProjectCode.Contains(request.search)));
        }
        // กรองปี
        if (!string.IsNullOrEmpty(request.Years))
        {
            if (int.TryParse(request.Years, out int year))
            {
                QueryActivityPlans = QueryActivityPlans.Where(x => x.StartDate.Year == year && x.EndDate.Year == year);
            }
        }
        // กรองเดือน
        if (request.Month.HasValue && request.Month.Value >= 1 && request.Month.Value <= 12)
        {
            QueryActivityPlans = QueryActivityPlans.Where(x =>
                (x.StartDate.Month <= request.Month.Value && x.EndDate.Month >= request.Month.Value) ||
                (x.StartDate.Month == request.Month.Value || x.EndDate.Month == request.Month.Value)
            );
        }
        if (request.DepartmentId != null && request.DepartmentId != Guid.Empty)
        {
            var employeeIds = _context.Employees
       .Where(e => e.DepartmentId == request.DepartmentId)
       .Select(e => e.Id)
       .ToList();

            // แล้วกรอง activity ที่อยู่ใน employee กลุ่มนี้
            QueryActivityPlans = QueryActivityPlans.Where(x => employeeIds.Contains(x.EmployeeId));

        }
        if (request.EmployeeId?.Any() == true)
        {
            QueryActivityPlans = QueryActivityPlans.Where(x => request.EmployeeId.Contains(x.EmployeeId));
        }
        #endregion
        var countitem = await QueryActivityPlans.CountAsync();
        var items = await QueryActivityPlans.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
              .Include(ap => ap.Employees)
        .Include(ap => ap.Projects)
        .Include(ap => ap.Organizations)
         .Include(ap => ap.ActivityPlanAttachments)
        .ThenInclude(at => at.Attachments)
        .Include(ap => ap.PlanNotes) // เพิ่ม Include ตรงนี้
              .Select(activity => new ActivityPlanExcelDto
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
                  Summary = activity.PlanNotes.Select(p => p.Summary).FirstOrDefault()!,
                  ToDoNext = activity.PlanNotes.Select(p => p.ToDoNext).FirstOrDefault(),
                  Remarks = activity.PlanNotes.Select(p => p.Remarks).FirstOrDefault(),
                  Images = activity.ActivityPlanAttachments != null
    ? activity.ActivityPlanAttachments
        .Select(att => att.Attachments)
        .ToList()
    : new List<Attachment>()
              }).ToListAsync(cancellationToken);

        return new PaginatedList<ActivityPlanExcelDto>(items, countitem, request.PageNumber, request.PageSize);
    }
}
