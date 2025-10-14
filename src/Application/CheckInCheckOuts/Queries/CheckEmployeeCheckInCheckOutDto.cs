using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Application.CheckInCheckOuts.Queries;
public class CheckEmployeeCheckInCheckOutDto
{
    public DateTime CurrentDate { get; set; }
    public bool Status { get; set; }
    public Guid? Id { get; set; }
}
