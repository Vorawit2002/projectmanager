using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.ActivityPlans.Commands.UpdateActivityPlan;
public class UpdateActivityPlanCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string? Objective { get; set; }
    public string? ObjectiveDetail { get; set; }
    public string? detail { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? OrganizationId { get; set; }
    public bool? AllDay { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Location { get; set; }
    public bool? HaveCost { get; set; }
    public string? CostDetail { get; set; }
    public decimal? Cost { get; set; }
    public bool? OutSide { get; set; }
}
public class UpdateActivityPlanCommandHandler : IRequestHandler<UpdateActivityPlanCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailSenderService _emailSenderService;
    public UpdateActivityPlanCommandHandler(IApplicationDbContext context, IEmailSenderService emailSenderService)
    {
        _context = context;
        _emailSenderService = emailSenderService;
    }
    public async Task<bool> Handle(UpdateActivityPlanCommand request, CancellationToken cancellationToken)
    {
        var activityPlans = await _context.ActivityPlans.Include(x => x.EventTypes).Include(x => x.Employees).FirstOrDefaultAsync(x => x.Id == request.Id);
        Guard.Against.NotFound(request.Id, activityPlans);
        var tenAMToday = DateTime.Today.AddHours(10);
        // Check if start and end date are the same day with 00:00 time, set as all-day event
        var isAllDayEvent = request.StartDate.TimeOfDay == TimeSpan.Zero &&
        request.EndDate.TimeOfDay == TimeSpan.Zero;
        activityPlans.EmployeeId = request.EmployeeId;
        activityPlans.Objective = request.Objective;
        activityPlans.ObjectiveDetail = request.ObjectiveDetail;
        activityPlans.detail = request.detail;
        activityPlans.ProjectId = request.ProjectId;
        activityPlans.OrganizationId = request.OrganizationId;
        activityPlans.StartDate = request.StartDate.ToLocalTime();
        activityPlans.EndDate = request.EndDate.ToLocalTime();
        activityPlans.AllDay = isAllDayEvent || request.AllDay.GetValueOrDefault();
        activityPlans.Location = request.Location;
        activityPlans.HaveCost = request.HaveCost;
        activityPlans.CostDetail = request.CostDetail;
        activityPlans.Cost = request.Cost;
        activityPlans.OutSide = request.OutSide;
        if (activityPlans.EventTypes!.EventTypeCode == "002" 
            && tenAMToday < DateTime.Now 
            && activityPlans.StartDate.Date <= DateTime.Now.Date && activityPlans.EndDate.Date >= DateTime.Now.Date)
        {
            BackgroundJob.Enqueue(() => _emailSenderService.SendEmailEditWorkSchedule(activityPlans.Employees!.Email, cancellationToken));
        }
        //await _context.ActivityPlans.AddAsync(activityPlans);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
