using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Entities;
public class EmailScheduleSetting : BaseAuditableEntity
{
    public string ScheduleName { get; set; } = default!;
    public string? HangfireJobId { get; set; }
    public Guid EmailMessageSettingId { get; set; }
    public virtual EmailMessageSetting EmailMessageSetting { get; set; } = default!;
    public DateTime SendMailDate { get; set; }
    public bool IsEnabled { get; set; }

}
