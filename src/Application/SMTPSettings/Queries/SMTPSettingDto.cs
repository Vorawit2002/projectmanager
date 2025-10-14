using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.SMTPSettings.Queries;
public class SMTPSettingDto
{
    public string Id { get; set; } = default!;
    public string? ConfigName { get; set; }
    public string? SMTPServer { get; set; }
    public string? SMTPPort { get; set; }
    public MailAuthenType? SMTPAuthentication { get; set; }
    public string? SMTPUserName { get; set; }
    public string? SMTPPassword { get; set; }
    public bool? SMTPEnableSSL { get; set; }
    public bool? IsActive { get; set; }
    public string? Remark { get; set; }


    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<SMTPSetting, SMTPSettingDto>();
        }
    }

}
