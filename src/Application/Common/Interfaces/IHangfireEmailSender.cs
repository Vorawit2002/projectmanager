using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;

namespace ProjectManagement.Application.Common.Interfaces;
public interface IHangfireEmailSender
{
    [JobDisplayName("ส่งเมล schedule")]
    Task EmailScheduleSend(Guid EmailScheduleSettingId, Guid EmailMessageSettingId);
}
