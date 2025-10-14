using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectManagement.Application.Common.Interfaces;

namespace ProjectManagement.Infrastructure;
public class DailyPushNotificationWorker : BackgroundService
{
    private readonly IServiceProvider _provider;

    public DailyPushNotificationWorker(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextRun = DateTime.Today.AddHours(16).AddMinutes(20);
            if (now > nextRun) nextRun = nextRun.AddDays(1);

            var delay = nextRun - now;
            await Task.Delay(delay, stoppingToken);

            using (var scope = _provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
                var pushService = scope.ServiceProvider.GetRequiredService<PushNotificationService>();

                var employees = await context.Employees.Where(e => e.Subscription == true).ToListAsync();
                foreach (var emp in employees)
                {
                    await pushService.SendPushNotificationsForEmployeeAsync(emp);
                }
            }
        }
    }
}
