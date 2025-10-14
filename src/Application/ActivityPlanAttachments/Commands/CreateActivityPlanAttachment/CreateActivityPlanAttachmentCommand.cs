using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.FileAttachments.Commands;

namespace ProjectManagement.Application.ActivityPlanAttachments.Commands.CreateActivityPlanAttachment;
public class CreateActivityPlanAttachmentCommand : IRequest<bool>
{
    public Guid ActivityPlanId { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public string Base64 { get; set; } = default!;
}
public class CreateActivityPlanAttachmentCommandHandler : IRequestHandler<CreateActivityPlanAttachmentCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ISender _sender;

    public CreateActivityPlanAttachmentCommandHandler(IApplicationDbContext context, ISender sender)
    {
        _sender = sender;
        _context = context;
    }
    public async Task<bool> Handle(CreateActivityPlanAttachmentCommand request, CancellationToken cancellationToken)
    {
        var Attachment = new ActivityPlanAttachment();
        var activityplan = await _context.ActivityPlans.Include(x => x.Employees).FirstOrDefaultAsync(x => x.Id == request.ActivityPlanId);
        Guard.Against.NotFound(request.ActivityPlanId, activityplan);
        Attachment.ActivityPlans = activityplan;

        var stream = new MemoryStream(Convert.FromBase64String(request.Base64));
        UploadAttachmentCommand command = new UploadAttachmentCommand();
        command.FileName = request.FileName;
        command.subdirectory = "activityplan";
        command.subindirectory = activityplan.Employees!.Email;
        command.FileStream = stream;
        var fileAttachment = await _sender.Send(command);
        if (fileAttachment != null)
        {
            Attachment.Attachments = fileAttachment;
        }

        await _context.ActivityPlanAttachments.AddAsync(Attachment);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
