using System;
using Hangfire;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Infrastructure;
using ProjectManagement.Infrastructure.Data;
using ProjectManagement.Web;
using WebPush;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
// Add services to the container.
builder.AddKeyVaultIfConfigured();
builder.AddApplicationServices();
builder.AddInfrastructureServices(builder.Configuration);
builder.AddWebServices();
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",
            "https://localhost:5173",
            "http://localhost:8098",
            "https://localhost:5001",
            "https://crm.nti.co.th"
        )
        .AllowCredentials()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
var app = builder.Build();
//GenerateVapidKeysIfNeeded();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
else
{
    await app.InitialiseDatabaseAsync();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseResponseCompression(); // Enable response compression
app.UseCors("AllowLocalhost"); // Apply CORS policy - must be before UseHttpsRedirection
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSwaggerUi(settings =>
{
    settings.Path = "/api";
    settings.DocumentPath = "/api/specification.json";
});

app.UseHangfireDashboard("/hangfire", new DashboardOptions()
{
    DarkModeEnabled = false,
    DisplayStorageConnectionString = false,
    IgnoreAntiforgeryToken = true,
    Authorization = new[] { new HangfireDashboardAuthorizationFilter() },

});

//BackgroundJob.Schedule<EmailSenderService>(
//    service => service.SendEmailWorkSchedule(CancellationToken.None),
//    TimeSpan.FromMinutes(2)
//);

//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//    var schedule = dbContext.EmailMessageSettings
//    .FirstOrDefault(s => s.Remark == "send-email-work-schedule" && s.IsActive);

//    if (schedule != null)
//    {
//        var recurringJobs = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
//        recurringJobs.AddOrUpdate<IEmailSenderService>(
//            schedule.Remark,
//            sender => sender.SendEmailWorkSchedule(CancellationToken.None),
//            schedule.Sendtime,
//            new RecurringJobOptions
//            {
//                TimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
//            });
//    }
//}

RecurringJob.RemoveIfExists("NotificationActivityPlan-001");
RecurringJob.RemoveIfExists("NotificationActivityPlan-002");

var tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

RecurringJob.AddOrUpdate<PushNotificationService>(
    "NotificationActivityPlan-001",
    s => s.SendPushNotificationsForAllEmployeesAsync(),
    "0 10 * * *", // 10:00 �.
    new RecurringJobOptions { TimeZone = tz });

RecurringJob.AddOrUpdate<PushNotificationService>(
    "NotificationActivityPlan-002",
    s => s.SendNotificationDailyScheduleForAllEmployeesAsync(),
    "30 09 * * *", // 10:17 �.
    new RecurringJobOptions { TimeZone = tz });

app.UseExceptionHandler(options => { });

app.Map("/", () => Results.Redirect("/api"));
app.MapRazorPages();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapEndpoints();

app.Run();
public partial class Program { }
