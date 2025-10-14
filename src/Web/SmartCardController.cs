using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ProjectManagement.Infrastructure;

namespace ProjectManagement.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SmartCardController : ControllerBase
    {
        private readonly SmartCardService _smartCardService;

        public SmartCardController(SmartCardService smartCardService)
        {
            _smartCardService = smartCardService;
        }

        [HttpGet("read")]
        public async Task<IActionResult> ReadCard()
        {
            try
            {
                Console.WriteLine("SmartCardController: Starting card read request");
                var data = await _smartCardService.GetCardDataAsync();
                
                if (data == null)
                {
                    Console.WriteLine("SmartCardController: No card data returned");
                    return BadRequest(new { 
                        message = "ไม่สามารถอ่านข้อมูลจากบัตรประจำตัวประชาชนได้",
                        details = "กรุณาตรวจสอบ: 1) บัตรใส่ถูกต้อง 2) เครื่องอ่านบัตรเชื่อมต่อแล้ว 3) Driver ติดตั้งแล้ว"
                    });
                }
                
                Console.WriteLine($"SmartCardController: Successfully read card data for CitizenId: {data.CitizenId}");
                return Ok(data);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"SmartCardController Card Read Error: {ex.Message}");
                return BadRequest(new { 
                    message = "เกิดข้อผิดพลาดในการอ่านบัตร",
                    details = ex.Message,
                    suggestions = new[]
                    {
                        "ตรวจสอบว่าบัตรใส่ถูกต้องและไม่เสียหาย",
                        "ลองถอดบัตรออกแล้วใส่ใหม่",
                        "ตรวจสอบการเชื่อมต่อเครื่องอ่านบัตร",
                        "รีสตาร์ทโปรแกรมและลองใหม่"
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SmartCardController Unexpected Error: {ex.Message}");
                Console.WriteLine($"SmartCardController Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { 
                    message = "เกิดข้อผิดพลาดที่ไม่คาดคิด",
                    error = ex.Message 
                });
            }
        }
    }
}
