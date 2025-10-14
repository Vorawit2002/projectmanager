using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Application.CheckInCheckOuts.Queries;
public class CheckLineUserIdCheckInCheckOutToDayDto
{
    public string LinebotUserId { get; set; } = default!;
    public bool IsFirstCheckinOfDay { get; set; }
    public DateTime CheckinTime { get; set; }
    public string Message { get; set; } = default!;
}
