using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.EmailScheduleSettings.Queries;
public class EmailScheduleSettingDto
{
    public Guid Id { get; set; }
    public Guid EmailMessageSettingId { get; set; }
    public virtual EmailMessageSetting EmailMessageSetting { get; set; } = default!;
    public DateTime SendMailDate { get; set; }
    public bool IsEnabled { get; set; }
    public string ScheduleName { get; set; } = default!;
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<EmailScheduleSetting, EmailScheduleSettingDto>();
        }
    }
}
