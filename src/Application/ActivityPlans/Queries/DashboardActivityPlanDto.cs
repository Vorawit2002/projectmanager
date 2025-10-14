using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Application.ActivityPlans.Queries;
public class DashboardActivityPlanDto
{
    public decimal CountSale { get; set; } // จำนวนงานนัดหมายของ sale 
    public decimal CountDevelopment { get; set; }// จำนวนงานนัดหมายของ dev
    public decimal CountProduct { get; set; }// จำนวนงานนัดหมายของ product
    public decimal CountSystemService { get; set; } // จำนวนงานนัดหมายของ system และ service 
    public decimal? Percent { get; set; } // อัตราการเติบโต (ไตรมาส ปี)
    public List<ObjectiveCountDto>? Objectives { get; set; } // รายการวัตถุประสงค์พร้อม count
    public decimal? Cost { get; set; } // ค่าใช้จ่าย
}
public class ObjectiveCountDto
{
    public string? Name { get; set; } // ชื่อวัตถุประสงค์ เช่น "ทานข้าว"
    public int Count { get; set; }    // จำนวนที่เจอ
}
