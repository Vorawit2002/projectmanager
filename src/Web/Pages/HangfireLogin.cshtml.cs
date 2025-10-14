using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;

namespace ProjectManagement.Web.Pages
{
    [IgnoreAntiforgeryToken] // เพิ่มเพื่อ bypass antiforgery สำหรับ login page
    public class HangfireLoginModel : PageModel
    {
        private readonly IAntiforgery _antiforgery;
        public HangfireLoginModel(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        [BindProperty]
        public string Username { get; set; } = default!;
        [BindProperty]
        public string Password { get; set; } = default!;

        public void OnGet()
        {
            // ลบ cookies เก่าเมื่อเข้าหน้า login
            //Response.Cookies.Delete("HangfireToken");
            //Response.Cookies.Delete(".AspNetCore.Antiforgery.TokenKey");
        }

        public IActionResult OnPost()
        {
            // Simple authentication check; replace with your logic
            if (Username == "admin" && Password == "hangfireP@ssw0rd")
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes("uba7036e7c9656a2e03c68d1be55c1033ccca5c88d4bbb3af4ca6ab687b4eb74");

                // Create JWT token with claims
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, Username) }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    Issuer = "crm.hangfire.Iss",
                    Audience = "crm.hangfire.Aud",
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                var domainName = Request.Host.Host;
                // Save the token to a cookie or local storage
                Response.Cookies.Append("HangfireToken", tokenString, new CookieOptions
                {
                    //HttpOnly = true,
                    //Secure = Request.IsHttps, // Auto-detect based on request
                    //SameSite = SameSiteMode.Lax,
                    //Path = "/",
                    //MaxAge = TimeSpan.FromHours(1)

                    HttpOnly = true,
                    Secure = true, // Ensure this is `false` for `http` or `true` for `https`
                    SameSite = SameSiteMode.Lax,
                    Path = "/",
                    MaxAge = TimeSpan.FromHours(1),
                    Domain = domainName // อย่าใส่ Domain ถ้าไม่จำเป็น — ให้ browser จัดการ
                });

                return Redirect("/hangfire"); // Redirect to Hangfire dashboard
            }


            // ลบ Cookie เก่าหากล็อกอินไม่สำเร็จ
            Response.Cookies.Delete("HangfireToken");
            //Response.Cookies.Delete(".AspNetCore.Antiforgery.<unique>");

            ViewData["Error"] = "Invalid credentials.";
            return Page();
        }
    }
}
