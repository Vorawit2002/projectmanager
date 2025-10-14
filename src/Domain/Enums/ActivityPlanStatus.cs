using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Enums;
public enum ActivityPlanStatus
{
    All = 0, // แสดงทั้งหมด
    Summary = 1, //สรุปรายงานแล้ว
    NotSummary = 2, //ยังไม่สรุปรายงาน
}
