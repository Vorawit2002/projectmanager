using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Application.CheckInCheckOuts.Queries;
public class CreateCheckInCheckOutDto
{
    public bool Status { get; set; }
    public string Message { get; set; } = default!;
}
