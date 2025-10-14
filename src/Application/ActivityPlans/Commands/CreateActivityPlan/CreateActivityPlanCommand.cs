using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.ActivityPlans.Commands.CreateActivityPlan;
public class CreateActivityPlanCommand : IRequest<Guid>
{
    public Guid EmployeeId { get; set; }
    public string? Objective { get; set; } 
    public string? ObjectiveDetail { get; set; }
    public string? detail { get; set; } 
    public Guid? ProjectId { get; set; }
    public Guid? OrganizationId { get; set; }
    public bool? AllDay { get; set; } = false;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Location { get; set; } 
    public bool? HaveCost { get; set; }
    public string? CostDetail { get; set; }
    public decimal? Cost { get; set; }
    public bool? OutSide { get; set; }
    public string EventType { get; set; } = default!;
}
public class CreateActivityPlanCommandHandler : IRequestHandler<CreateActivityPlanCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailSenderService _emailSenderService;
    public CreateActivityPlanCommandHandler(IApplicationDbContext context, IEmailSenderService emailSenderService)
    {
        _context = context;
        _emailSenderService = emailSenderService;
    }
    public async Task<Guid> Handle(CreateActivityPlanCommand request, CancellationToken cancellationToken)
    {
        var eventType = await _context.EventTypes.FirstOrDefaultAsync(x => x.EventTypeCode == request.EventType);
        Guard.Against.NotFound(request.EventType, eventType);
        var emp = await _context.Employees.FirstOrDefaultAsync(x => x.Id == request.EmployeeId);
        Guard.Against.NotFound(request.EmployeeId, emp);
        
        // Check if start and end date are the same day with 00:00 time, set as all-day event
        var isAllDayEvent = request.StartDate.TimeOfDay == TimeSpan.Zero &&
                           request.EndDate.TimeOfDay == TimeSpan.Zero;
        
        var tenAMToday = DateTime.Today.AddHours(10);
        var activityPlans = new ActivityPlan();
        activityPlans.EmployeeId = request.EmployeeId;
        activityPlans.Objective = request.Objective;
        activityPlans.ObjectiveDetail = request.ObjectiveDetail;
        activityPlans.detail = request.detail;
        activityPlans.ProjectId = request.ProjectId;
        activityPlans.OrganizationId = request.OrganizationId;
        activityPlans.AllDay = isAllDayEvent || request.AllDay.GetValueOrDefault();
        activityPlans.Location = request.Location;
        activityPlans.HaveCost = request.HaveCost;
        activityPlans.CostDetail = request.CostDetail;
        activityPlans.Cost = request.Cost;
        activityPlans.OutSide = request.OutSide;
        activityPlans.EventTypes = eventType;
        var startDate = request.StartDate.ToLocalTime();
        var endDate = request.EndDate.ToLocalTime();
        //if (eventType.EventTypeCode == "002")
        //{
            //var activityCheckDay = await _context.ActivityPlans
            //    .Include(x => x.EventTypes)
            //    .FirstOrDefaultAsync(x => x.StartDate.Date == startDate.Date
            //    && x.EventTypeId == eventType.Id
            //    && x.EmployeeId == request.EmployeeId);
            //if (activityCheckDay != null)
            //{
            //    return Guid.Empty;
            //}
            //if (startDate.Date != endDate.Date)
            //{
            //    var createdActivityIds = new List<Guid>();

            //    DateTime currentDate = startDate.Date;
            //    DateTime endDates = endDate.Date;

            //    while (currentDate <= endDate)
            //    {
            //        // ข้ามวันเสาร์และอาทิตย์
            //        if (currentDate.DayOfWeek != DayOfWeek.Saturday &&
            //            currentDate.DayOfWeek != DayOfWeek.Sunday)
            //        {
            //            // สร้าง ActivityPlan object ใหม่สำหรับแต่ละวัน
            //            var dailyActivityPlan = new ActivityPlan
            //            {
            //                EmployeeId = request.EmployeeId,
            //                Objective = request.Objective,
            //                ObjectiveDetail = request.ObjectiveDetail,
            //                detail = request.detail,
            //                ProjectId = request.ProjectId,
            //                OrganizationId = request.OrganizationId,
            //                AllDay = isAllDayEvent || request.AllDay.GetValueOrDefault(),
            //                Location = request.Location,
            //                HaveCost = request.HaveCost,
            //                CostDetail = request.CostDetail,
            //                Cost = request.Cost,
            //                OutSide = request.OutSide,
            //                EventTypes = eventType,
            //                StartDate = currentDate.ToLocalTime(),
            //                EndDate = currentDate.ToLocalTime()
            //            };

            //            await _context.ActivityPlans.AddAsync(dailyActivityPlan);
            //            createdActivityIds.Add(dailyActivityPlan.Id);
            //        }

            //        currentDate = currentDate.AddDays(1);
            //    }

            //    await _context.SaveChangesAsync(cancellationToken);

            //    // ส่งกลับ ID ของ ActivityPlan แรกที่สร้าง
            //    return createdActivityIds.FirstOrDefault();
            //}
            //else
            //{
                //activityPlans.StartDate = request.StartDate.ToLocalTime();
                //activityPlans.EndDate = request.EndDate.ToLocalTime();
                //await _context.ActivityPlans.AddAsync(activityPlans);
                //await _context.SaveChangesAsync(cancellationToken);
                //if (tenAMToday < DateTime.Now && DateTime.Now.Date == startDate.Date)
                //{
                //    BackgroundJob.Enqueue(() => _emailSenderService.SendEmailEditWorkSchedule(emp.Email, cancellationToken));
                //}
                //return activityPlans.Id;
            //}
        //}
        //else
        //{
            activityPlans.StartDate = request.StartDate.ToLocalTime();
            activityPlans.EndDate = request.EndDate.ToLocalTime();
            await _context.ActivityPlans.AddAsync(activityPlans);
            await _context.SaveChangesAsync(cancellationToken);
            if (tenAMToday < DateTime.Now && DateTime.Now.Date == startDate.Date)
            {
                BackgroundJob.Enqueue(() => _emailSenderService.SendEmailEditWorkSchedule(emp.Email, cancellationToken));
            }
            return activityPlans.Id;
        //}
    }
}
