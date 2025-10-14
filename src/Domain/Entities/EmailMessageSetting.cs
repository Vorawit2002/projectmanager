using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Entities;
public class EmailMessageSetting : BaseAuditableEntity
{
    public string SettingCode { get; set; } = default!;
    public string SettingName { get; set; } = default!;
    public string MailSubject { get; set; } = default!;
    public string MailBody { get; set; } = default!;
    public bool IsActive { get; set; } = default!;
    public string? Remark { get; set; }
    public string? Sendtime { get; set; }
}
