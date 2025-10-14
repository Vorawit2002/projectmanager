using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;

namespace ProjectManagement.Application.Common.Interfaces;
public interface IEmailSenderService
{
    [JobDisplayName("อีเมลแจ้งตารางงาน")]
    Task SendEmailWorkSchedule( CancellationToken cancellationToken);
    [JobDisplayName("อีเมลแจ้งแก้ไขตารางงาน")]
    Task SendEmailEditWorkSchedule(string email ,CancellationToken cancellationToken);
    [JobDisplayName("ส่งเมล")]
    Task SendEmailAsync(string email, string subject, string message, CancellationToken cancellationToken);
    [JobDisplayName("ส่งอีเมลแชร์การนัดหมาย")]
    Task SendEmailShareActivityPlan(Guid Id,string title, string url, string senderEmail, string? remarks, CancellationToken cancellationToken);
}
