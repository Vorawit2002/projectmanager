using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.EmailLogs.Commands.CreateEmailLog;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using Hangfire;
using Hangfire.States;

namespace ProjectManagement.Application.EmailScheduleSettings.Commands.CreateEmailScheduleSetting;
public class CreateEmailScheduleSettingCommand : IRequest<bool>
{
    public string ScheduleName { get; set; } = default!;
    public Guid EmailMessageSettingId { get; set; }
    public DateTime SendMailDate { get; set; }
    public bool IsEnabled { get; set; }
}

public class CreateEmailScheduleSettingCommandHandler : IRequestHandler<CreateEmailScheduleSettingCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ISender _sender;
    private readonly IHangfireEmailSender _hangfireEmailSender;
    public CreateEmailScheduleSettingCommandHandler(IApplicationDbContext context, ISender sender, IHangfireEmailSender hangfireEmailSender)
    {
        _context = context;
        _sender = sender;
        _hangfireEmailSender = hangfireEmailSender;
    }
    public async Task<bool> Handle(CreateEmailScheduleSettingCommand request, CancellationToken cancellationToken)
    {
        var emailScheduleSetting = new EmailScheduleSetting();

        var emailMessageSetting = await _context.EmailMessageSettings.FirstOrDefaultAsync(x => x.Id == request.EmailMessageSettingId);
        Guard.Against.NotFound(request.EmailMessageSettingId, emailMessageSetting);
        emailScheduleSetting.EmailMessageSetting = emailMessageSetting;

        emailScheduleSetting.SendMailDate = request.SendMailDate.ToLocalTime(); ;
        emailScheduleSetting.IsEnabled = request.IsEnabled;
        emailScheduleSetting.ScheduleName = request.ScheduleName;

        await _context.EmailScheduleSettings.AddAsync(emailScheduleSetting);
        await _context.SaveChangesAsync(cancellationToken);

        if(emailScheduleSetting.IsEnabled)
        {
            TimeSpan delay = emailScheduleSetting.SendMailDate - DateTime.Now;
            string jobId = emailScheduleSetting.HangfireJobId??string.Empty;
            if (!string.IsNullOrEmpty(jobId))
            {
                using (var connection = JobStorage.Current.GetConnection())
                {
                    var job = connection.GetJobData(jobId);
                    if (job.State != SucceededState.StateName)
                    {
                        if (job != null) // The job exists
                        {
                            // Delete the existing job
                            BackgroundJob.Delete(jobId);
                        }
                    }
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
            emailScheduleSetting.HangfireJobId = BackgroundJob.Schedule(() => _hangfireEmailSender.EmailScheduleSend(emailScheduleSetting.Id, emailMessageSetting.Id), delay);
        }
        return true;
    }
}
