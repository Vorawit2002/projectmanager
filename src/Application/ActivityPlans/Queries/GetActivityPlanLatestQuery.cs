using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.ActivityPlans.Commands.CreateActivityPlan;
public class GetActivityPlanLatestQuery : IRequest<Guid?>
{
    public Guid EmployeeId { get; set; }
}
public class GetActivityPlanLatestQueryHandler : IRequestHandler<GetActivityPlanLatestQuery, Guid?>
{
    private readonly IApplicationDbContext _context;

    public GetActivityPlanLatestQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Guid?> Handle(GetActivityPlanLatestQuery request, CancellationToken cancellationToken)
    {
        var activityPlansYesterDay = await _context.ActivityPlans.Include(x => x.EventTypes).Where(x => x.EmployeeId == request.EmployeeId 
        && x.EventTypes!.EventTypeCode == "002")
      .OrderByDescending(x => x.EndDate)
    .FirstOrDefaultAsync();
        if (activityPlansYesterDay != null)
        {
            return activityPlansYesterDay.Id;
        }
        else
        {
            return null;
        }
    }
}
