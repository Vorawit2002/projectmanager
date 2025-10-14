using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Hangfire;
using ProjectManagement.Application.ActivityPlans.Commands.CreateActivityPlan;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.ActivityPlans.Commands.ShareActivityPlan;
public class ShareActivityPlanCommand : IRequest<bool>
{
    public Guid ActivityPlanId { get; set; }
    public string Title { get; set; } = default!;
    public IList<string>? Emails { get; set; }
    public string? Remarks { get; set; }
}
public class ShareActivityPlanCommandHandler : IRequestHandler<ShareActivityPlanCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailSenderService _emailSenderService;
    public ShareActivityPlanCommandHandler(IApplicationDbContext context, IEmailSenderService emailSenderService)
    {
        _context = context;
        _emailSenderService = emailSenderService;
    }
    public async Task<bool> Handle(ShareActivityPlanCommand request, CancellationToken cancellationToken)
    {
        var activityPlans = await _context.ActivityPlans.Include(x => x.EventTypes).Include(x => x.Employees).FirstOrDefaultAsync(x => x.Id == request.ActivityPlanId);
        Guard.Against.NotFound(request.ActivityPlanId, activityPlans);
        string returnUrl = $"/activity/{activityPlans.Id}/shared";

        // 1) สร้าง state (+ เก็บ csrf ลง cookie เพื่อตรวจตอน callback)
        var (state, csrf) = OidcStateHelper.CreateState(returnUrl);
        var url = "https://ntiportal.nti.co.th/connect/authorize?response_type=code&client_id=OPNNricj2qWgVQqo3x4JjHpaoDy6Z0&redirect_uri=https://crm.nti.co.th/login-callback&scope=openid profile email roles phone profile_image&code_challenge=gEKX6x8KW3Pxfna9viyf6ZHhTZlleNA15rxji0jWlvM&code_challenge_method=S256&state="+ state;
        if (request.Emails != null && request.Emails.Any())
        {
            foreach (var email in request.Emails)
            {
                BackgroundJob.Enqueue(() => _emailSenderService.SendEmailShareActivityPlan(activityPlans.Id,request.Title, url, email, request.Remarks, cancellationToken));
            }
            return true;
        }
        else
        {
            return false;
        }
    }
    public record OidcState(string csrf, string ru);
    public static class OidcStateHelper
    {
        // สร้างค่า state จาก returnUrl (ru) และ random csrf
        public static (string state, string csrf) CreateState(string returnUrl)
        {
            var csrf = CreateCsrf();                       // สุ่ม CSRF token
            var payload = new OidcState(csrf, returnUrl);  // { "csrf": "...", "ru": "..." }
            var json = JsonSerializer.Serialize(payload);
            var state = Base64UrlEncode(Encoding.UTF8.GetBytes(json));
            return (state, csrf);
        }

        // แตก state เป็น object (ใช้ตอน callback เพื่อตรวจ csrf และอ่าน ru)
        public static OidcState? ParseState(string state)
        {
            if (string.IsNullOrEmpty(state)) return null;
            var bytes = Base64UrlDecode(state);
            var json = Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<OidcState>(json);
        }

        // ===== helper =====
        private static string CreateCsrf()
        {
            // สร้างค่า random 32 ไบต์แล้วเข้ารหัส base64url
            Span<byte> buf = stackalloc byte[32];
            RandomNumberGenerator.Fill(buf);
            return Base64UrlEncode(buf.ToArray());
        }

        public static string Base64UrlEncode(byte[] bytes)
        {
            var s = Convert.ToBase64String(bytes);
            s = s.TrimEnd('=').Replace('+', '-').Replace('/', '_');
            return s;
        }

        public static byte[] Base64UrlDecode(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/');
            switch (s.Length % 4)
            {
                case 2: s += "=="; break;
                case 3: s += "="; break;
            }
            return Convert.FromBase64String(s);
        }
    }
}
