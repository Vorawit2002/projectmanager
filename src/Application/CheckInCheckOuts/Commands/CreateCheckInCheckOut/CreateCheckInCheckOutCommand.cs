using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ProjectManagement.Application.CheckInCheckOuts.Queries;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.FileAttachments.Commands;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.CheckInCheckOuts.Commands.CreateCheckInCheckOut;
public class CreateCheckInCheckOutCommand : IRequest<CreateCheckInCheckOutDto>
{
    public Guid EmployeeId { get; set; }
    public string LatLong { get; set; } = default!;
    public string? Location { get; set; }
    public string? IPAddress { get; set; }
    public DateTime CheckIn { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid? ProjectId { get; set; }
    public string FileName { get; set; } = default!;
    public string Base64 { get; set; } = default!;
    public CheckInCheckOutType? CheckInCheckOutTypes { get; set; }
}
public class CreateCheckInCheckOutCommandHandler : IRequestHandler<CreateCheckInCheckOutCommand, CreateCheckInCheckOutDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailSenderService _emailSenderService;
    private readonly ISender _sender;
    public CreateCheckInCheckOutCommandHandler(IApplicationDbContext context, IEmailSenderService emailSenderService, ISender sender)
    {
        _context = context;
        _emailSenderService = emailSenderService;
        _sender = sender;
    }
    public async Task<CreateCheckInCheckOutDto> Handle(CreateCheckInCheckOutCommand request, CancellationToken cancellationToken)
    {
        var Checkinout = new CheckInCheckOut();
        var dto = new CreateCheckInCheckOutDto();
        var emp = await _context.Employees.FirstOrDefaultAsync(x => x.Id == request.EmployeeId);
        Guard.Against.NotFound(request.EmployeeId, emp);
        var parts = request.LatLong.Split(',');

        if (request.OrganizationId != null && request.OrganizationId != Guid.Empty)
        {
            var organization = await _context.Organizations.FirstOrDefaultAsync(x => x.Id == request.OrganizationId);
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
                Checkinout.OrganizationId = request.OrganizationId;
            }
        }
        Checkinout.Lat = parts[0];
        Checkinout.Long = parts[1];
        Checkinout.Employees = emp;
        Checkinout.IPAddress = request.IPAddress;
        Checkinout.Location = request.Location;
        Checkinout.CheckIn = request.CheckIn.ToLocalTime();
        Checkinout.ProjectId = request.ProjectId;
        Checkinout.Types = "Default";
        string typeCheckInCheckOut = "CheckIn";
        if (request.CheckInCheckOutTypes == CheckInCheckOutType.CheckOut)
        {
            typeCheckInCheckOut = "CheckOut";
        }
        Checkinout.CheckInCheckOutTypes = request.CheckInCheckOutTypes;
        await _context.CheckInCheckOuts.AddAsync(Checkinout);
        await _context.SaveChangesAsync(cancellationToken);
        var checkinoutAttachment = new CheckInCheckOutAttachment();
        var stream = new MemoryStream(Convert.FromBase64String(request.Base64));
        UploadAttachmentCommand command = new UploadAttachmentCommand();
        command.FileName = request.FileName;
        command.subdirectory = "CheckInCheckOut";
        command.subindirectory = emp.Email + " " + typeCheckInCheckOut;
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
}
