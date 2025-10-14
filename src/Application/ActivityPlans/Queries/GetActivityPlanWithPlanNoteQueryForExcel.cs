using System;
using System.Collections.Generic;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ProjectManagement.Application.ActivityPlans.Queries;

public record GetActivityPlanWithPlanNoteQueryForExcel : IRequest<IEnumerable<ActivityPlanExcelDto>>;
public class GetActivityPlanWithPlanNoteQueryForExcelHandler : IRequestHandler<GetActivityPlanWithPlanNoteQueryForExcel, IEnumerable<ActivityPlanExcelDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetActivityPlanWithPlanNoteQueryForExcelHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<ActivityPlanExcelDto>> Handle(GetActivityPlanWithPlanNoteQueryForExcel request, CancellationToken cancellationToken)
    {
        var QueryActivityPlans = _context.ActivityPlans
        .Include(ap => ap.Employees)
        .Include(ap => ap.Projects)
        .Include(ap => ap.Organizations)
        .Include(ap => ap.EventTypes)
        .Include(ap => ap.PlanNotes) // เพิ่ม Include ตรงนี้
        .Include(ap => ap.ActivityPlanAttachments)
        .ThenInclude(at => at.Attachments)
        .Where(ap => ap.PlanNotes.Any() && ap.EventTypes!.EventTypeCode == "001")
        .OrderBy(ap => ap.StartDate)
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
          })
        .ToListAsync();

        return await QueryActivityPlans;
    }
}
