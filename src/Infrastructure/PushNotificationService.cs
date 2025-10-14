using System.Net;
using System.Reflection;
using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProjectManagement.Application.ActivityPlans.Queries;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using WebPush;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

public class PushNotificationService
{
    private readonly IApplicationDbContext _context;
    private readonly VapidDetails _vapid;
    private readonly ISender _sender;

    public PushNotificationService(IApplicationDbContext context, IConfiguration config, ISender sender)
    {
        _sender = sender;
        _context = context;
        _vapid = new VapidDetails(
            config["PushNotifications:Subject"],
            config["PushNotifications:PublicKey"],
            config["PushNotifications:PrivateKey"]
        );
    }

    public async Task SendPushNotificationsForEmployeeAsync(Employee employee)
    {
        var cancellationToken = CancellationToken.None; // หรือ CancellationTokenSource.Token
        if (employee.Subscription == null || employee.Subscription == false) return; // ถ้าไม่ subscribe ก็ไม่ส่ง

        // ดึง notification
        var notificationsquery = new GetNotificationActivityPlanByEmployeeIdQuery();
        notificationsquery.employeeId = employee.Id;
       var notifications  = await _sender.Send(notificationsquery);
        // ดึง subscriptions ของ employee
        var subscriptions = await _context.PushSubscriptions
            .Where(x => x.EmployeeId == employee.Id)
            .ToListAsync();

        var client = new WebPushClient();

        foreach (var subEntity in subscriptions.ToList()) // .ToList() กัน modify ระหว่าง loop
        {
            // ใช้ ctor ที่ถูกต้องของ WebPush.PushSubscription
            var pushSub = new WebPush.PushSubscription(subEntity.Endpoint, subEntity.P256dh, subEntity.Auth);

            foreach (var notif in notifications)
            {
                var payload = JsonSerializer.Serialize(new
                {
                    title = notif.Title,
                    message = notif.Objective,
                    url = "/activityplan"
                });

                try
                {
                    // ✅ ใช้ VapidDetails ที่เตรียมไว้ (อย่าใช้ Dictionary)
                    await client.SendNotificationAsync(pushSub, payload, _vapid);
                }
                catch (WebPushException ex) when (ex.StatusCode == HttpStatusCode.Gone || ex.StatusCode == HttpStatusCode.NotFound)
                {
                    
                    _context.PushSubscriptions.Remove(subEntity);
                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (WebPushException ex)
                {
                    Console.WriteLine($"Push failed ({(int)ex.StatusCode} {ex.StatusCode}): {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Push error: {ex.Message}");
                }
            }
        }

    }

    public async Task SendPushNotificationsForAllEmployeesAsync()
    {
        var employees = await _context.Employees.Where(e => e.Subscription == true).ToListAsync();
        foreach (var emp in employees)
        {
            await SendPushNotificationsForEmployeeAsync(emp);
        }
    }


    // ส่งการแจ้งเตือนการเข้างาน
    public async Task SendNotificationDailyScheduleAsync(Employee employee)
    {
        var cancellationToken = CancellationToken.None; // หรือ CancellationTokenSource.Token
        if (employee.Subscription == null || employee.Subscription == false) return; // ถ้าไม่ subscribe ก็ไม่ส่ง

        // ดึง notification
        var notifications = await _context.ActivityPlans
            .Where(x => x.EmployeeId == employee.Id && (x.StartDate.Date == DateTime.Now.Date || (x.StartDate.Date <= DateTime.Now.Date && x.EndDate.Date >= DateTime.Now.Date)))
            .ToListAsync();
        // ดึง subscriptions ของ employee
        var subscriptions = await _context.PushSubscriptions
            .Where(x => x.EmployeeId == employee.Id)
            .ToListAsync();

        var client = new WebPushClient();
        if(!notifications.Any()) { 
        foreach (var subEntity in subscriptions.ToList()) // .ToList() กัน modify ระหว่าง loop
        {
            // ใช้ ctor ที่ถูกต้องของ WebPush.PushSubscription
            var pushSub = new WebPush.PushSubscription(subEntity.Endpoint, subEntity.P256dh, subEntity.Auth);
                var payload = JsonSerializer.Serialize(new
                {
                    title = "แจ้งเตือนการลงงาน",
                    message = "กรุณาเข้าสู่ระบบเพื่อแจ้งงานรายวัน",
                    url = "/CustomerAppointmentListView"
                });
                try
                {
                    // ✅ ใช้ VapidDetails ที่เตรียมไว้ (อย่าใช้ Dictionary)
                    await client.SendNotificationAsync(pushSub, payload, _vapid);
                }
                catch (WebPushException ex) when (ex.StatusCode == HttpStatusCode.Gone || ex.StatusCode == HttpStatusCode.NotFound)
                {
                    _context.PushSubscriptions.Remove(subEntity);
                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (WebPushException ex)
                {
                    Console.WriteLine($"Push failed ({(int)ex.StatusCode} {ex.StatusCode}): {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Push error: {ex.Message}");
                }
            }
        }

    }

    public async Task SendNotificationDailyScheduleForAllEmployeesAsync()
    {
        var employees = await _context.Employees.Where(e => e.Subscription == true).ToListAsync();
        foreach (var emp in employees)
        {
            await SendNotificationDailyScheduleAsync(emp);
        }
    }
}
