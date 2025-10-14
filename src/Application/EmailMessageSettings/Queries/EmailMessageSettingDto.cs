using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.EmailMessageSettings.Queries;
public class EmailMessageSettingDto
{
    public string Id { get; set; } = default!;
    public string SettingCode { get; set; } = default!;
    public string SettingName { get; set; } = default!;
  //  public string? BccMails { get; set; } = default!;
    public string MailSubject { get; set; } = default!;
    public string MailBody { get; set; } = default!;
    public bool? IsActive { get; set; }
    public string? Remark { get; set; }


    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<EmailMessageSetting, EmailMessageSettingDto>();
        }
    }

}
