using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.FileAttachments.Commands;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.CheckInCheckOuts.Commands.UpdateCheckInCheckOut;
public class UpdateCheckInCheckOutCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    //public string? Location { get; set; }
    //public string? IPAddress { get; set; }
    public DateTime CheckOut { get; set; }
    public string LocationCheckOut { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public string Base64 { get; set; } = default!;
}
public class UpdateCheckInCheckOutCommandHandler : IRequestHandler<UpdateCheckInCheckOutCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailSenderService _emailSenderService;
    private readonly ISender _sender;
    public UpdateCheckInCheckOutCommandHandler(IApplicationDbContext context, IEmailSenderService emailSenderService, ISender sender)
    {
        _context = context;
        _emailSenderService = emailSenderService;
        _sender = sender;
    }
    public async Task<bool> Handle(UpdateCheckInCheckOutCommand request, CancellationToken cancellationToken)
    {
        var Checkinout = await _context.CheckInCheckOuts.Include(x => x.Employees).FirstOrDefaultAsync(x => x.Id == request.Id);
        Guard.Against.NotFound(request.Id, Checkinout);
        //Checkinout.IPAddress = request.IPAddress;
        //Checkinout.Location = request.Location;
        Checkinout.CheckOut = request.CheckOut.ToLocalTime();
        Checkinout.LocationCheckOut = request.LocationCheckOut;
        var checkinoutAttachment = new CheckInCheckOutAttachment();
        var stream = new MemoryStream(Convert.FromBase64String(request.Base64));
        UploadAttachmentCommand command = new UploadAttachmentCommand();
        command.FileName = request.FileName;
        command.subdirectory = "CheckInCheckOut";
        command.subindirectory = Checkinout.Employees!.Email  +" CheckOut";
        command.FileStream = stream;
        var fileAttachment = await _sender.Send(command);
        if (fileAttachment != null)
        {
            checkinoutAttachment.Attachments = fileAttachment;
        }
        checkinoutAttachment.CheckInCheckOutId = Checkinout.Id;
        checkinoutAttachment.Type = CheckInCheckOutType.CheckOut;
        await _context.CheckInCheckOutAttachments.AddAsync(checkinoutAttachment);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
