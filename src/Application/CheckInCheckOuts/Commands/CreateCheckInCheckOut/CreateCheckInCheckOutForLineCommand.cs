using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using ProjectManagement.Application.CheckInCheckOuts.Queries;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.FileAttachments.Commands;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.CheckInCheckOuts.Commands.CreateCheckInCheckOut;
public class CreateCheckInCheckOutForLineCommand : IRequest<CreateCheckInCheckOutDto>
{
    public string UserId { get; set; } = default!;
    public string LatLong { get; set; } = default!;
    public string? Location { get; set; }
    public string? IPAddress { get; set; }
    public string? OrganizationName { get; set; }
    public DateTime CheckInCheckOutTime { get; set; }
    public string? ProjectName { get; set; }
    public string FileName { get; set; } = default!;
    public string Base64 { get; set; } = default!;
    public CheckInCheckOutType? CheckInCheckOutTypes { get; set; }
}
public class CreateCheckInCheckOutForLineCommandHandler : IRequestHandler<CreateCheckInCheckOutForLineCommand, CreateCheckInCheckOutDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailSenderService _emailSenderService;
    private readonly ISender _sender;
    private readonly HttpClient _httpClient;

    public CreateCheckInCheckOutForLineCommandHandler(IApplicationDbContext context, IEmailSenderService emailSenderService, ISender sender, HttpClient httpClient)
    {
        _context = context;
        _emailSenderService = emailSenderService;
        _sender = sender;
        _httpClient = httpClient;
    }
    public async Task<CreateCheckInCheckOutDto> Handle(CreateCheckInCheckOutForLineCommand request, CancellationToken cancellationToken)
    {
        var Checkinout = new CheckInCheckOut();
        var dto = new CreateCheckInCheckOutDto();
        string url = $"https://ntiportal.nti.co.th/GetUserid?LineUserId={request.UserId}";
        ApiResponse? result = null;
        try
        {
            if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", "ApiKey ntk_be7e07a95cf6406396fb4ac48757aea3");
            }
            using HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string jsonResult = await response.Content.ReadAsStringAsync();
            Console.WriteLine("ผลลัพธ์จาก API: " + jsonResult);

            // แปลง JSON เป็น object
            result = JsonConvert.DeserializeObject<ApiResponse>(jsonResult);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("เกิดข้อผิดพลาดในการเรียก API: " + ex.Message);
            throw new InvalidOperationException(ex.Message);
        }
        var parts = request.LatLong.Split(',');
        if (result != null && !string.IsNullOrEmpty(result.id))
        {
            var emp = await _context.Employees.FirstOrDefaultAsync(x => x.UserId == result.id!);
            Guard.Against.NotFound(result.id, emp);

            //var toDate = DateTime.Now.ToLocalTime();
            if (request.OrganizationName != null && request.OrganizationName != string.Empty)
            {
                var organization = await _context.Organizations.FirstOrDefaultAsync(x => x.Name.Contains(request.OrganizationName));
                if (organization != null)
                {
                   
                    var latLongOrganization = organization.Coordinates!.Split(',');

                    double distance = HaversineDistance(latLongOrganization[0], latLongOrganization[1], parts[0], parts[1]);

                    if (distance > 2) // ถ้าเกิน 2 กิโลเมตร
                    {
                        dto.Status = false;
                        dto.Message = "พิกัดไม่ตรงกับหน่วยงานที่บันทึกไว้ กรุณาอยู่ในขอบเขตของหน่วยงาน";
                        return dto;
                    }
                    Checkinout.OrganizationId = organization.Id;
                }
            }
            if (request.ProjectName != null && request.ProjectName != string.Empty)
            {
                var projects = await _context.Projects.FirstOrDefaultAsync(x => x.ProjectName.Contains(request.ProjectName));
                if (projects != null)
                {

                    Checkinout.ProjectId = projects.Id;
                }
            }
            Checkinout.Lat = parts[0];
            Checkinout.Long = parts[1];
            Checkinout.Employees = emp;
            Checkinout.IPAddress = request.IPAddress;
            Checkinout.Location = request.Location;
            Checkinout.CheckIn = request.CheckInCheckOutTime.ToLocalTime();
            Checkinout.CheckInCheckOutTypes = request.CheckInCheckOutTypes;
            Checkinout.Types = "Line";
            string typeCheckInCheckOut = "CheckIn";
            if (request.CheckInCheckOutTypes == CheckInCheckOutType.CheckOut)
            {
                typeCheckInCheckOut = "CheckOut";
            }
            await _context.CheckInCheckOuts.AddAsync(Checkinout);
            await _context.SaveChangesAsync(cancellationToken);
            var checkinoutAttachment = new CheckInCheckOutAttachment();
            var stream = new MemoryStream(Convert.FromBase64String(request.Base64));
            UploadAttachmentCommand command = new UploadAttachmentCommand();
            command.FileName = request.FileName;
            command.subdirectory = "CheckInCheckOut";
            command.subindirectory = emp.Email +" "+ typeCheckInCheckOut;
            ;
            command.FileStream = stream;
            var fileAttachment = await _sender.Send(command);
            if (fileAttachment != null)
            {
                checkinoutAttachment.Attachments = fileAttachment;
            }

            checkinoutAttachment.CheckInCheckOutId = Checkinout.Id;
            checkinoutAttachment.Type = CheckInCheckOutType.CheckIn;
            await _context.CheckInCheckOutAttachments.AddAsync(checkinoutAttachment);
            await _context.SaveChangesAsync(cancellationToken);
            dto.Status = true;
            dto.Message = Checkinout.Id.ToString();
            return dto;
        }
        else
        {
            dto.Status = false;
            dto.Message = "ไม่เจอค่าUserId";
            return dto;
        }
    }

private double HaversineDistance(string lat1Str, string lon1Str, string lat2Str, string lon2Str)
    {
        const double R = 6371; // รัศมีของโลก (กิโลเมตร)

        double lat1 = double.Parse(lat1Str.Trim());
        double lon1 = double.Parse(lon1Str.Trim());
        double lat2 = double.Parse(lat2Str.Trim());
        double lon2 = double.Parse(lon2Str.Trim());

        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }

    private double ToRadians(double angle)
    {
        return angle * Math.PI / 180.0;
    }
    public class ApiResponse
    {
        public string message { get; set; } = string.Empty;
        public string id { get; set; } = string.Empty;
        public string linebotUserId { get; set; } = string.Empty;
        public string userName { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
    }
}
