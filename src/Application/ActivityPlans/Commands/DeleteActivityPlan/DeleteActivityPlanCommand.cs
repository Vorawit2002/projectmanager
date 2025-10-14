using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.ActivityPlans.Commands.DeleteActivityPlan;
public record DeleteActivityPlanCommand(Guid Id) : IRequest<bool>;
public class DeleteActivityPlanCommandHandler : IRequestHandler<DeleteActivityPlanCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteActivityPlanCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(DeleteActivityPlanCommand request, CancellationToken cancellationToken)
    {
           var activityPlans = await _context.ActivityPlans.FirstOrDefaultAsync(x=>x.Id == request.Id);
            Guard.Against.NotFound(request.Id, activityPlans);
         _context.ActivityPlans.Remove(activityPlans);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
