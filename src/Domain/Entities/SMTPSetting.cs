using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Domain.Entities
{
    public class SMTPSetting : BaseAuditableEntity
    {
        public string ConfigName { get; set; } = default!;
        public string SMTPServer { get; set; } = default!;
        public string SMTPPort { get; set; } = default!;
        public MailAuthenType SMTPAuthentication { get; set; }
        public string SMTPUserName { get; set; } = default!;
        public string SMTPPassword { get; set; } = default!;
        public bool SMTPEnableSSL { get; set; } = default!;
        public bool IsActive { get; set; }
        public string? Remark { get; set; } = default!;

    }
}
