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

namespace ProjectManagement.Application.OrganizationContacts.Commands.UpdateOrganizationContact;
public class UpdateOrganizationContactCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string TitleName { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? Position { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? LineId { get; set; }
    public string? FileName { get; set; }
    public string? Base64 { get; set; }
}
public class UpdateOrganizationContactCommandHandler : IRequestHandler<UpdateOrganizationContactCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ISender _sender;
    public UpdateOrganizationContactCommandHandler(IApplicationDbContext context, ISender sender)
    {
        _context = context;
        _sender = sender;
    }
    public async Task<bool> Handle(UpdateOrganizationContactCommand request, CancellationToken cancellationToken)
    {
        var organizationContacts = await _context.OrganizationContacts.FirstOrDefaultAsync(x => x.Id == request.Id);
        Guard.Against.NotFound(request.Id, organizationContacts);
        organizationContacts.OrganizationId = request.OrganizationId;
        organizationContacts.TitleName = request.TitleName;
        organizationContacts.FirstName = request.FirstName;
        organizationContacts.LastName = request.LastName;
        organizationContacts.Position = request.Position;
        organizationContacts.Email = request.Email;
        organizationContacts.Phone = request.Phone;
        organizationContacts.Fax = request.Fax;
        organizationContacts.LineId = request.LineId;
        if (!string.IsNullOrEmpty(request.Base64) && !string.IsNullOrEmpty(request.FileName))
        {
            var stream = new MemoryStream(Convert.FromBase64String(request.Base64));
            UploadAttachmentCommand command = new UploadAttachmentCommand();
            command.FileName = request.FileName;
            command.subdirectory = "organizationcontacts";
            command.subindirectory =request.TitleName + request.FirstName + " " +request.LastName;
            command.FileStream = stream;
            var fileAttachment = await _sender.Send(command);
            if (fileAttachment != null)
            {
                organizationContacts.Attachments = fileAttachment;
            }
        }
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
