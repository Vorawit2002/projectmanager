using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reactive.Joins;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjectManagement.Application.ActivityPlanAttachments.Queries;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.EmailLogs.Commands.CreateEmailLog;
using ProjectManagement.Domain.Entities;
using static ServiceStack.Diagnostics.Events;

namespace ProjectManagement.Infrastructure;
public class EmailSenderService : IEmailSenderService
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<EmailSenderService> _logger;
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ISender _sender;
    public EmailSenderService(IConfiguration configuration, IApplicationDbContext context, ILogger<EmailSenderService> logger, UserManager<ApplicationUser> userManager, ISender sender)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
        _userManager = userManager;
        _sender = sender;
    }

    [JobDisplayName("อีเมลแจ้งตารางงาน")]
    public async Task SendEmailWorkSchedule(CancellationToken cancellationToken)
    {
        var emailMessageSetting = await _context.EmailMessageSettings.FirstOrDefaultAsync(x => x.SettingCode == "001" && x.IsActive == true);
        if (emailMessageSetting != null)
        {
            CultureInfo thaiCulture = new CultureInfo("th-TH");
            // พนักงานทุกคนที่มีอีเมล
            var allEmployees = await _context.Employees
                .Include(e => e.Departments)
                .Where(e => !string.IsNullOrEmpty(e.Email))
                .ToListAsync(cancellationToken);

            // ดึง ActivityPlan ที่เป็นของวันนี้ และเป็น "ตารางงานประจำวัน"
            var activityPlans = await _context.ActivityPlans
                .Include(x => x.Employees)
                .ThenInclude(e => e!.Departments)
                .Include(x => x.EventTypes)
                .Include(x => x.Organizations)
                .Include(x => x.Projects)
                .Where(x => x.EventTypes != null &&
                   (x.StartDate.Date == DateTime.Now.Date || (x.StartDate.Date <= DateTime.Now.Date && x.EndDate.Date >= DateTime.Now.Date)))
                .ToListAsync(cancellationToken);

            // ✅ เช็คว่ามี ActivityPlan หรือไม่ ถ้าไม่มีให้ return ทันที
            if (activityPlans.Count == 0)
            {
                return; // ไม่ส่งอีเมลเลยถ้าไม่มีใครแจ้งงาน
            }

            // สร้างรายการที่รวมพนักงานทุกคน และจับคู่กับ activity ทั้งหมดที่ตรงกับเขา (อาจมากกว่า 1 รายการ)
            var employeePlans = allEmployees.Select(emp =>
            {
                var activities = activityPlans
                    .Where(a => a.Employees != null && a.Employees.Id == emp.Id)
                    .OrderBy(x => x.StartDate)
                    .ToList();

                return new
                {
                    Employee = emp,
                    Plans = activities // List<ActivityPlan> อาจว่าง ถ้าคนนั้นไม่มี plan
                };
            }).ToList();

            // Group by department
            var groupByDepartment = employeePlans
                .Where(e => e.Employee.Departments != null)
                .GroupBy(e => e.Employee.Departments!.Name)
                 .Where(g => g.Any(x => x.Plans.Any()));

            //// ดึงอีเมลของพนักงานทุกคน
            var emailsAll = await _context.Employees
            .Where(p => !string.IsNullOrEmpty(p.Email))
            .Select(p => p.Email)
            .Distinct()
            .ToListAsync(cancellationToken);
            string currentDate = DateTime.Now.ToString("dd MMMM yyyy", new CultureInfo("th-TH"));
            if (activityPlans.Count > 0)
            {
                foreach (var departmentGroup in groupByDepartment)
                {
                    string departmentName = departmentGroup.Key;
                    var plans = departmentGroup
                    .OrderBy(x => x.Employee.FirstName, StringComparer.Create(new CultureInfo("th-TH"), false))
                    .ToList();

                    var rowsHtml = new StringBuilder();
                    int index = 1;

                    foreach (var item in plans)
                    {
                        var emp = item.Employee;
                        var empPlans = item.Plans;

                        if (empPlans.Any())
                        {

                            string rowColor = index % 2 == 0 ? "#edfbfc" : "#f0f8fc";

                            var type001Plans = empPlans.Where(p => p.EventTypes?.EventTypeCode == "001").ToList();
                            var type002Plans = empPlans.Where(p => p.EventTypes?.EventTypeCode == "002").ToList();

                            string type001Details = "";
                            string type002Details = "";

                            // สำหรับ EventType "001" - แสดงเวลาและรายละเอียด
                            if (type001Plans.Any())
                            {
                                type001Details = string.Join("<br>", type001Plans.Select(plan =>
                                {
                                    string timeInfo = plan.AllDay == true
                                        ? " ทั้งวัน "
                                        : plan.StartDate.Date == plan.EndDate.Date ? plan.StartDate.ToString(" H:mm", thaiCulture) + "-" + plan.EndDate.ToString("H:mm", thaiCulture) :
                                        plan.StartDate.ToString("d MMMM yyyy H:mm", thaiCulture) + "-" + plan.EndDate.ToString("d MMMM yyyy H:mm", thaiCulture);

                                    string rawObjective = plan.Objective!.Contains("อื่น") ? (plan.ObjectiveDetail ?? "") : (plan.Objective ?? "");
                                    string objective = (rawObjective ?? "").Replace("\r\n", "<br>").Replace("\n", "<br>");
                                    string organization = (plan.Organizations!.ShortName ?? plan.Organizations!.Name ?? "").Replace("\r\n", "<br>").Replace("\n", "<br>");

                                    return $"<li><strong>{timeInfo} : {objective} ({organization})</strong></li>";
                                }));
                            }

                            // สำหรับ EventType "002" - แสดงเฉพาะรายละเอียด
                            if (type002Plans.Any())
                            {
                                type002Details = string.Join("<br>", type002Plans.Select(plan =>
                                {
                                    string detail = (plan.detail ?? "").Replace("\r\n", "<br>").Replace("\n", "<br>");
                                    return $"{detail}";
                                }));
                            }

                            // รวมทั้งสองส่วน โดยมีเส้นแบ่งถ้ามีทั้งสองประเภท
                            string allDetails = "";
                            if (!string.IsNullOrEmpty(type001Details) && !string.IsNullOrEmpty(type002Details))
                            {
                                allDetails = "<ul>" + type001Details + "</ul>" + "<hr style='border:0;border-top:1px solid #ccc; margin:8px 0;'>" + type002Details;
                            }
                            else if (!string.IsNullOrEmpty(type001Details))
                            {
                                allDetails = "<ul>" + type001Details + "</ul>";
                            }
                            else if (!string.IsNullOrEmpty(type002Details))
                            {
                                allDetails = type002Details;
                            }
                            else
                            {
                                allDetails = "-";
                            }

                            string projectInfo = empPlans.Any()
                                ? string.Join("<br>", empPlans.Select(p => p.Projects != null ? $"{p.Projects.ProjectCode} {p.Projects.ShortName ?? ""}" : ""))
                                : "";

                            rowsHtml.AppendLine($@"
                                <tr style='background-color: {rowColor}; transition: background-color 0.2s;'>
                                 <td style='padding: 12px 10px; text-align: center; border: 1px solid #ddd; border-bottom: 1px solid #ccc; font-weight: bold; color: #495057; border-left: 3px solid #2196f3;'>{index++}</td>
                                <td style='padding: 12px 10px; border: 1px solid #ddd; border-bottom: 1px solid #ccc; color: #212529;' >
                                 <div style='display: flex; align-items: center;'>
                                <strong>{emp.TitleName} {emp.FirstName} {emp.LastName}</strong>
                                </div>
                                </td>
                                <td style='padding: 12px 10px; border: 1px solid #ddd; border-bottom: 1px solid #ccc; color: #495057; line-height: 1.4; padding: 8px 12px; border-radius: 6px; '>
                                {allDetails}
                                </td>
                                <td style='padding: 12px 10px; text-align: center; border: 1px solid #ddd; border-bottom: 1px solid #ccc; color: #495057;'>
                                {projectInfo}
                                 </td>
                                </tr>");

                        }
                        else
                        {
                            // ❗ ถ้าไม่มีแผน
                            rowsHtml.AppendLine($@"
                            <tr style='background-color: #fff3cd;'>
                            <td style='padding: 12px 10px; text-align: center; border: 1px solid #ddd;'>{index++}</td>
                            <td colspan='3' style='padding: 12px 10px; border: 1px solid #ddd; color: #856404;'>
                            {emp.TitleName} {emp.FirstName} {emp.LastName} - ไม่มีตารางงานวันนี้
                            </td>
                            </tr>");
                        }
                    }

                    string finalBody = emailMessageSetting.MailBody
                        .Replace("{rows}", rowsHtml.ToString())
                        .Replace("{date}", currentDate)
                        .Replace("{department}", departmentName);

                    string subjectWithDepartment = emailMessageSetting.MailSubject
                        .Replace("{department}", departmentName)
                        .Replace("{date}", currentDate);



                    await SendEmailListAsync(emailsAll, subjectWithDepartment, finalBody, cancellationToken);
                }
            }

        }
    }

    [JobDisplayName("อีเมลแจ้งแก้ไขตารางงาน")]
    public async Task SendEmailEditWorkSchedule(string senderEmail, CancellationToken cancellationToken)
    {
        var emailMessageSetting = await _context.EmailMessageSettings.FirstOrDefaultAsync(x => x.SettingCode == "002" && x.IsActive == true);
        if (emailMessageSetting != null)
        {
            CultureInfo thaiCulture = new CultureInfo("th-TH");
            // พนักงานทุกคนที่มีอีเมล
            var allEmployees = await _context.Employees
                .Include(e => e.Departments)
                .Where(e => !string.IsNullOrEmpty(e.Email))
                .ToListAsync(cancellationToken);

            // ดึง ActivityPlan ที่เป็นของวันนี้
            var activityPlans = await _context.ActivityPlans
                .Include(x => x.Employees)
                .ThenInclude(e => e!.Departments)
                .Include(x => x.Organizations)
                .Include(x => x.EventTypes)
                .Include(x => x.Projects)
                .Where(x => x.EventTypes != null &&
                   (x.StartDate.Date == DateTime.Now.Date || (x.StartDate.Date <= DateTime.Now.Date && x.EndDate.Date >= DateTime.Now.Date))
                   )
                .ToListAsync(cancellationToken);

            // STEP 1: หาคนที่ส่งเมลมา
            var sender = await _context.Employees
                .Include(e => e.Departments)
                .FirstOrDefaultAsync(e => e.Email == senderEmail, cancellationToken);

            if (sender == null || sender.Departments == null)
                return; // ไม่เจอ sender หรือไม่มีแผนก

            var senderDepartmentName = sender.Departments.Name;
            // สร้างรายการที่รวมพนักงานทุกคน และจับคู่กับ activity ทั้งหมดที่ตรงกับเขา (อาจมากกว่า 1 รายการ)
            var employeePlans = allEmployees.Select(emp =>
            {
                var activities = activityPlans
                    .Where(a => a.Employees != null && a.Employees.Id == emp.Id)
                    .OrderBy(x => x.StartDate)
                    .ToList();

                return new
                {
                    Employee = emp,
                    Plans = activities // List<ActivityPlan> อาจว่าง ถ้าคนนั้นไม่มี plan
                };
            }).ToList();
            // STEP 2: กรองเฉพาะแผนกของ sender
            var groupByDepartment = employeePlans
                .Where(e => e.Employee.Departments != null && e.Employee.Departments.Name == senderDepartmentName)
                .GroupBy(e => e.Employee.Departments!.Name);

            //// ดึงอีเมลของพนักงานทุกคน
            var emailsAll = await _context.Employees
            .Where(p => !string.IsNullOrEmpty(p.Email))
            .Select(p => p.Email)
            .Distinct()
            .ToListAsync(cancellationToken);
            string currentDate = DateTime.Now.ToString("dd MMMM yyyy", new CultureInfo("th-TH"));
            if (activityPlans.Count > 0)
            {
                foreach (var departmentGroup in groupByDepartment)
                {
                    string departmentName = departmentGroup.Key;
                    var plans = departmentGroup
                    .OrderBy(x => x.Employee.FirstName, StringComparer.Create(new CultureInfo("th-TH"), false))
                    .ToList();

                    var rowsHtml = new StringBuilder();
                    int index = 1;

                    foreach (var item in plans)
                    {
                        var emp = item.Employee;
                        var plansItem = item.Plans;
                        if (plansItem.Any())
                        {

                            string editColor = "#FFFDE7";
                            bool isSender = emp.Email == senderEmail;
                            string headerText = isSender ? "<div style = 'display: flex; align-items: start; color:red;'><strong>ปรับแก้ตารางงาน</strong> </div><br>" : "";
                            string rowColor = isSender ? editColor : (index % 2 == 0 ? "#edfbfc" : "#f0f8fc");
                            var type001Plans = plansItem.Where(p => p.EventTypes?.EventTypeCode == "001").ToList();
                            var type002Plans = plansItem.Where(p => p.EventTypes?.EventTypeCode == "002").ToList();

                            string type001Details = "";
                            string type002Details = "";

                            // สำหรับ EventType "001" - แสดงเวลาและรายละเอียด
                            if (type001Plans.Any())
                            {
                                type001Details = string.Join("<br>", type001Plans.Select(plan =>
                                {
                                    string timeInfo = plan.AllDay == true
                                        ? " ทั้งวัน "
                                        : plan.StartDate.Date == plan.EndDate.Date ? plan.StartDate.ToString(" H:mm", thaiCulture) + "-" + plan.EndDate.ToString("H:mm", thaiCulture) :
                                        plan.StartDate.ToString("d MMMM yyyy H:mm", thaiCulture) + "-" + plan.EndDate.ToString("d MMMM yyyy H:mm", thaiCulture);
                                    ;

                                    string rawObjective = plan.Objective == "อื่น ๆ" ? (plan.ObjectiveDetail ?? "") : (plan.Objective ?? "");
                                    string objective = (rawObjective ?? "").Replace("\r\n", "<br>").Replace("\n", "<br>");
                                    string organization = (plan.Organizations!.ShortName ?? plan.Organizations!.Name ?? "").Replace("\r\n", "<br>").Replace("\n", "<br>");
                                    return $"<strong>{timeInfo} : {objective} ({organization})</strong>";
                                }));
                            }

                            // สำหรับ EventType "002" - แสดงเฉพาะรายละเอียด
                            if (type002Plans.Any())
                            {
                                type002Details = string.Join("<br>", type002Plans.Select(plan =>
                                {
                                    string detail = (plan.detail ?? "").Replace("\r\n", "<br>").Replace("\n", "<br>");
                                    return $"{detail}";
                                }));
                            }

                            // รวมทั้งสองส่วน โดยมีเส้นแบ่งถ้ามีทั้งสองประเภท
                            string allDetails = "";
                            if (!string.IsNullOrEmpty(type001Details) && !string.IsNullOrEmpty(type002Details))
                            {
                                allDetails = "<ul>" + type001Details + "</ul>" + "<hr style='border:0;border-top:1px solid #ccc; margin:8px 0;'>" + type002Details;
                            }
                            else if (!string.IsNullOrEmpty(type001Details))
                            {
                                allDetails = "<ul>" + type001Details + "</ul>";
                            }
                            else if (!string.IsNullOrEmpty(type002Details))
                            {
                                allDetails = type002Details;
                            }
                            else
                            {
                                allDetails = "-";
                            }


                            // ใช้ project จาก plan แรก หรือรวมทุก project ถ้าต้องการ
                            //string projectInfo = plansItem.FirstOrDefault()?.Projects != null
                            //    ? $"{plansItem.FirstOrDefault()!.Projects!.ProjectCode} {plansItem.FirstOrDefault()!.Projects!.ProjectName}"
                            //    : "-";

                            // ถ้าต้องการแสดงทุก project ให้ใช้นี้แทน:
                            ///*
                            string projectInfo = string.Join(", ", plansItem
                                .Where(p => p.Projects != null)
                                .Select(p => $"{p.Projects!.ProjectCode} {p.Projects!.ShortName ?? ""}")
                                .Distinct());
                            if (string.IsNullOrEmpty(projectInfo)) projectInfo = "";
                            //*/

                            rowsHtml.AppendLine($@"
            <tr style='background-color: {rowColor}; transition: background-color 0.2s;'>
                <td style='padding: 12px 10px; text-align: center; border: 1px solid #ddd; border-bottom: 1px solid #ccc; font-weight: bold; color: #495057; border-left: 3px solid #2196f3;'>{index++}</td>
                <td style='padding: 12px 10px; border: 1px solid #ddd; border-bottom: 1px solid #ccc; color: #212529;' >
                    <div style='display: flex; align-items: center;'>
                        <strong>{emp.TitleName} {emp.FirstName} {emp.LastName}</strong>
                    </div>
                </td>
                <td style='padding: 12px 10px; border: 1px solid #ddd; border-bottom: 1px solid #ccc; color: #495057; line-height: 1.4; padding: 8px 12px; border-radius: 6px; '>
                    {headerText}{allDetails}
                </td>
                <td style='padding: 12px 10px; text-align: center; border: 1px solid #ddd; border-bottom: 1px solid #ccc; color: #495057;'>
                    {projectInfo}
                </td>
            </tr>");

                        }
                        else
                        {
                            // ❗ ถ้าไม่มีแผน
                            rowsHtml.AppendLine($@"
                            <tr style='background-color: #fff3cd;'>
                            <td style='padding: 12px 10px; text-align: center; border: 1px solid #ddd;'>{index++}</td>
                            <td colspan='3' style='padding: 12px 10px; border: 1px solid #ddd; color: #856404;'>
                            {emp.TitleName} {emp.FirstName} {emp.LastName} - ไม่มีตารางงานวันนี้
                            </td>
                            </tr>");
                        }
                    }

                    string finalBody = emailMessageSetting.MailBody
                        .Replace("{rows}", rowsHtml.ToString())
                        .Replace("{date}", currentDate)
                        .Replace("{department}", departmentName);

                    string subjectWithDepartment = emailMessageSetting.MailSubject
                        .Replace("{department}", departmentName)
                        .Replace("{date}", currentDate);



                    await SendEmailListAsync(emailsAll, subjectWithDepartment, finalBody, cancellationToken);
                }
            }

        }
    }

    [JobDisplayName("ส่งเมล")]
    public async Task SendEmailAsync(string email, string subject, string body, CancellationToken cancellationToken)
    {
        var sMTPSetting = await _context.SMTPSettings.FirstOrDefaultAsync(x => x.IsActive == true);
        Guard.Against.NotFound("sMTPSetting", sMTPSetting);
        using (var client = new SmtpClient(sMTPSetting.SMTPServer, Convert.ToInt16(sMTPSetting.SMTPPort)))
        {
            client.UseDefaultCredentials = false;
            client.EnableSsl = sMTPSetting.SMTPEnableSSL;
            client.Credentials = new NetworkCredential(sMTPSetting.SMTPUserName, sMTPSetting.SMTPPassword);
            client.Port = Convert.ToInt32(sMTPSetting.SMTPPort);
            var mailMessage = new MailMessage
            {
                From = new MailAddress(sMTPSetting.SMTPUserName ?? "", sMTPSetting.SMTPUserName ?? ""),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            //if (!string.IsNullOrEmpty(Bcc))
            //{
            //    var emailList = Bcc.Split(',').Select(email => email.Trim());

            //    foreach (var emailBcc in emailList)
            //    {
            //        if (!string.IsNullOrWhiteSpace(emailBcc))
            //        {
            //            mailMessage.Bcc.Add(Bcc);
            //        }
            //    }
            //}

            mailMessage.To.Add(email);

            try
            {
                await client.SendMailAsync(mailMessage);

                CreateEmailLogCommand command = new CreateEmailLogCommand();
                command.Subject = subject;
                command.SentTo = email;
                command.Massage = body;
                command.SendStatus = true;
                command.SendBy = "system";
                await _sender.Send(command);

                _logger.LogInformation("send e-mail success");
            }
            catch (SmtpException smtpEx)
            {
                // Log SMTP-specific issues
                CreateEmailLogCommand command = new CreateEmailLogCommand();
                command.Subject = subject;
                command.SentTo = email;
                command.Massage = body;
                command.SendStatus = false;
                command.SendBy = "system";
                await _sender.Send(command);

                throw new InvalidOperationException($"SMTP Error: {smtpEx.Message}");
            }
            catch (Exception ex)
            {
                // Handle the error
                throw new InvalidOperationException(ex.Message);
            }

        }
    }
    [JobDisplayName("ส่งเมลแบบหลายคน")]
    public async Task SendEmailListAsync(List<string> emails, string subject, string body, CancellationToken cancellationToken)
    {
        var sMTPSetting = await _context.SMTPSettings.FirstOrDefaultAsync(x => x.IsActive == true);
        Guard.Against.NotFound("sMTPSetting", sMTPSetting);

        using (var client = new SmtpClient(sMTPSetting.SMTPServer, Convert.ToInt16(sMTPSetting.SMTPPort)))
        {
            client.UseDefaultCredentials = false;
            client.EnableSsl = sMTPSetting.SMTPEnableSSL;
            client.Credentials = new NetworkCredential(sMTPSetting.SMTPUserName, sMTPSetting.SMTPPassword);
            client.Port = Convert.ToInt32(sMTPSetting.SMTPPort);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(sMTPSetting.SMTPUserName ?? "", sMTPSetting.SMTPUserName ?? ""),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.CC.Add("navarat@nti.co.th");
            // ✅ ใส่อีเมลใน BCC
            foreach (var email in emails)
            {
                mailMessage.Bcc.Add(email);
            }

            try
            {
                await client.SendMailAsync(mailMessage);

                foreach (var email in emails)
                {
                    var command = new CreateEmailLogCommand
                    {
                        Subject = subject,
                        SentTo = email,
                        Massage = body,
                        SendStatus = true,
                        SendBy = "system"
                    };
                    await _sender.Send(command);
                }

                _logger.LogInformation("send e-mail success");
            }
            catch (SmtpException smtpEx)
            {
                foreach (var email in emails)
                {
                    var command = new CreateEmailLogCommand
                    {
                        Subject = subject,
                        SentTo = email,
                        Massage = body,
                        SendStatus = false,
                        SendBy = "system"
                    };
                    await _sender.Send(command);
                }

                throw new InvalidOperationException($"SMTP Error: {smtpEx.Message}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
    }

    [JobDisplayName("ส่งอีเมลแชร์การนัดหมาย")]
    public async Task SendEmailShareActivityPlan(Guid Id, string title, string url, string senderEmail, string? remarks, CancellationToken cancellationToken)
    {
        var sMTPSetting = await _context.SMTPSettings.FirstOrDefaultAsync(x => x.IsActive == true);
        Guard.Against.NotFound("sMTPSetting", sMTPSetting);

        var activityPlans = await _context.ActivityPlans
            .Include(x => x.EventTypes)
            .Include(x => x.Employees)
            .Include(x => x.PlanNotes)
            .FirstOrDefaultAsync(x => x.Id == Id);
        Guard.Against.NotFound(Id, activityPlans);

        var emailMessageSetting = await _context.EmailMessageSettings
            .FirstOrDefaultAsync(x => x.SettingCode == "003" && x.IsActive == true);

        if (emailMessageSetting != null)
        {
            using (var client = new SmtpClient(sMTPSetting.SMTPServer, Convert.ToInt16(sMTPSetting.SMTPPort)))
            {
                client.UseDefaultCredentials = false;
                client.EnableSsl = sMTPSetting.SMTPEnableSSL;
                client.Credentials = new NetworkCredential(sMTPSetting.SMTPUserName, sMTPSetting.SMTPPassword);
                client.Port = Convert.ToInt32(sMTPSetting.SMTPPort);

                // สร้าง HTML สำหรับรูปภาพ
                var imgTags = new StringBuilder();
                var activityPlansAttachmentDtos = await _context.ActivityPlanAttachments
                    .Include(x => x.Attachments)
                    .Include(x => x.ActivityPlans)
                    .Where(l => l.ActivityPlanId == activityPlans.Id)
                    .ToListAsync();

                var imagePaths = activityPlansAttachmentDtos
                    .Where(x => !string.IsNullOrEmpty(x.Attachments?.PathFile))
                    .Select(x => x.Attachments.PathFile)
                    .ToList();

                // สร้าง HTML สำหรับรูปภาพตามจำนวน
                if (imagePaths.Count > 0)
                {
                    if (imagePaths.Count == 1)
                    {
                        // แสดงรูปเดียวแบบเต็ม宽度
                        imgTags.Append($@"
                        <div style='text-align:center; margin-bottom:20px;'>
                            <img src='{imagePaths[0]}' alt='รูปภาพประกอบ' style='max-width:100%; height:auto; border:1px solid #eaeaea;' />
                        </div>");
                    }
                    else if (imagePaths.Count <= 4)
                    {
                        // แสดงรูปเป็น grid 2x2 โดยใช้ตาราง
                        imgTags.Append(@"<table width='100%' cellpadding='0' cellspacing='0' border='0' style='margin-bottom:20px;'>");

                        for (int i = 0; i < imagePaths.Count; i++)
                        {
                            if (i % 2 == 0) imgTags.Append("<tr>");

                            imgTags.Append($@"
                            <td width='50%' style='padding:5px; text-align:center;'>
                                <img src='{imagePaths[i]}' alt='รูปภาพ {i + 1}' style='max-width:100%; height:auto; border:1px solid #eaeaea;' />
                            </td>");

                            if (i % 2 == 1 || i == imagePaths.Count - 1) imgTags.Append("</tr>");
                        }

                        imgTags.Append("</table>");
                    }
                    else
                    {
                        // ถ้ามากกว่า 4 รูป ให้แสดงแค่ 4 รูปแรกและลิงก์ดูทั้งหมด
                        imgTags.Append(@"<table width='100%' cellpadding='0' cellspacing='0' border='0' style='margin-bottom:20px;'>");

                        for (int i = 0; i < 4; i++)
                        {
                            if (i % 2 == 0) imgTags.Append("<tr>");

                            imgTags.Append($@"
                            <td width='50%' style='padding:5px; text-align:center;'>
                                <img src='{imagePaths[i]}' alt='รูปภาพ {i + 1}' style='max-width:100%; height:auto; border:1px solid #eaeaea;' />
                            </td>");

                            if (i % 2 == 1) imgTags.Append("</tr>");
                        }

                        imgTags.Append("</table>");

                        // เพิ่มลิงก์ดูรูปภาพทั้งหมด
                        imgTags.Append($@"
                        <div style='background:#f8f9fa; padding:20px; text-align:center; margin-bottom:20px;'>
                            <p style='margin:0 0 15px 0; color:#5a6c7d;'>มีรูปภาพทั้งหมด {imagePaths.Count} รูป</p>
                            <a href='{url}' style='display:inline-block; background:#667eea; color:#ffffff; text-decoration:none; padding:12px 24px; font-size:14px; font-weight:bold;'>
                                ดูรูปภาพทั้งหมด
                            </a>
                        </div>");
                    }
                }

                string finalBody = emailMessageSetting.MailBody
                    .Replace("{images}", imgTags.ToString())
                    .Replace("{todonext}", activityPlans.PlanNotes.FirstOrDefault()?.ToDoNext ?? "")
                    .Replace("{summary}", activityPlans.PlanNotes.FirstOrDefault()?.Summary ?? "")
                    .Replace("{url}", url)
                    .Replace("{note}", remarks ?? "");

                string subjectWithDepartment = emailMessageSetting.MailSubject
                    .Replace("{title}", title);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(sMTPSetting.SMTPUserName ?? "", sMTPSetting.SMTPUserName ?? ""),
                    Subject = subjectWithDepartment,
                    Body = finalBody,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(senderEmail);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
