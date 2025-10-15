using Hangfire;
using ProjectManagement.Application.ActivityPlans.Commands.CreateActivityPlan;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.ActivityPlans.Commands.ShareActivityPlan;
public class ShareActivityPlanCommand : IRequest<bool>
{
    public Guid ActivityPlanId { get; set; }
    public string Title { get; set; } = default!;
    public IList<string>? Emails { get; set; }
    public string? Remarks { get; set; }
}
public class ShareActivityPlanCommandHandler : IRequestHandler<ShareActivityPlanCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailSenderService _emailSenderService;
    public ShareActivityPlanCommandHandler(IApplicationDbContext context, IEmailSenderService emailSenderService)
    {
        _context = context;
        _emailSenderService = emailSenderService;
    }
    public async Task<bool> Handle(ShareActivityPlanCommand request, CancellationToken cancellationToken)
    {
        var activityPlans = await _context.ActivityPlans.Include(x => x.EventTypes).Include(x => x.Employees).FirstOrDefaultAsync(x => x.Id == request.ActivityPlanId);
        Guard.Against.NotFound(request.ActivityPlanId, activityPlans);
        
        // Create local share link
        var url = $"/activity/{activityPlans.Id}/shared";
        
        if (request.Emails != null && request.Emails.Any())
        {
            foreach (var email in request.Emails)
            {
                BackgroundJob.Enqueue(() => _emailSenderService.SendEmailShareActivityPlan(activityPlans.Id, request.Title, url, email, request.Remarks, cancellationToken));
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}
