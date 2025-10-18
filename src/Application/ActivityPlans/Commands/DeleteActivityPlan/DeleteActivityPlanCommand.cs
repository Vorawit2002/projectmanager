using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Exceptions;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.ActivityPlans.Commands.DeleteActivityPlan;
public record DeleteActivityPlanCommand(Guid Id) : IRequest<bool>;
public class DeleteActivityPlanCommandHandler : IRequestHandler<DeleteActivityPlanCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IDataFilterService _dataFilterService;

    public DeleteActivityPlanCommandHandler(
        IApplicationDbContext context,
        IUser currentUser,
        IIdentityService identityService,
        IDataFilterService dataFilterService)
    {
        _context = context;
        _currentUser = currentUser;
        _identityService = identityService;
        _dataFilterService = dataFilterService;
    }
    
    public async Task<bool> Handle(DeleteActivityPlanCommand request, CancellationToken cancellationToken)
    {
        var activityPlans = await _context.ActivityPlans
            .Include(x => x.Employees)
            .FirstOrDefaultAsync(x=>x.Id == request.Id);
        Guard.Against.NotFound(request.Id, activityPlans);
        
        // Check authorization
        var userId = _currentUser.Id ?? throw new UnauthorizedAccessException();
        var roles = (await _identityService.GetUserRolesAsync(userId)).ToArray();
        
        var canAccess = await _dataFilterService.CanAccessResourceAsync(
            userId, 
            roles, 
            activityPlans.CreatedBy,
            activityPlans.Employees?.DepartmentId);
            
        if (!canAccess)
        {
            throw new ForbiddenAccessException();
        }
        
        _context.ActivityPlans.Remove(activityPlans);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
