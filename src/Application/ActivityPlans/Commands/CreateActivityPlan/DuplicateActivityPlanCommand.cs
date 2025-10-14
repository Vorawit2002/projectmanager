using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.ActivityPlans.Commands.CreateActivityPlan;
public class DuplicateActivityPlanCommand: IRequest<bool>
{
    public Guid ActivityId { get; set; }
}
public class DuplicateActivityPlanCommandHandler : IRequestHandler<DuplicateActivityPlanCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DuplicateActivityPlanCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(DuplicateActivityPlanCommand request, CancellationToken cancellationToken)
    {
        var activityPlansYesterDay = await _context.ActivityPlans.Include(x => x.EventTypes).Where(x => x.Id == request.ActivityId
        && x.EventTypes!.EventTypeCode == "002")
        .OrderByDescending(x => x.StartDate)
        .FirstOrDefaultAsync();
        Guard.Against.NotFound(request.ActivityId, activityPlansYesterDay);
        var activityPlans = new ActivityPlan();
        activityPlans.EmployeeId = activityPlansYesterDay.EmployeeId;
        activityPlans.detail = activityPlansYesterDay.detail;
        activityPlans.ProjectId = activityPlansYesterDay.ProjectId;
        activityPlans.StartDate = activityPlansYesterDay.StartDate.ToLocalTime().AddDays(1);
        activityPlans.EndDate = activityPlansYesterDay.EndDate.ToLocalTime().AddDays(1);
        activityPlans.AllDay = activityPlansYesterDay.AllDay;
        activityPlans.EventTypeId = activityPlansYesterDay.EventTypeId;


        await _context.ActivityPlans.AddAsync(activityPlans);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
      
    }
}
