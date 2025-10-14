using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Infrastructure;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Application;
public class HangfireEmailSender : IHangfireEmailSender
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailSenderService _emailSenderService;
    public HangfireEmailSender(IApplicationDbContext context,IEmailSenderService emailSenderService)
    {
        _context = context;
        _emailSenderService = emailSenderService;
    }

    [JobDisplayName("ส่งเมล schedule")]
    public async Task EmailScheduleSend(Guid EmailScheduleSettingId ,Guid EmailMessageSettingId)
    {
        var emailScheduleSetting = await _context.EmailScheduleSettings.FirstOrDefaultAsync(x => x.IsEnabled == true && x.Id == EmailScheduleSettingId);
        Guard.Against.Null(emailScheduleSetting);
        var emailMessageSetting = await _context.EmailMessageSettings.FirstOrDefaultAsync(x => x.IsActive == true && x.Id == EmailMessageSettingId);
        Guard.Against.Null(emailMessageSetting);
        await _emailSenderService.SendEmailAsync("temp@gmail.com", emailMessageSetting.MailSubject, emailMessageSetting.MailBody,  CancellationToken.None);


    }
}
