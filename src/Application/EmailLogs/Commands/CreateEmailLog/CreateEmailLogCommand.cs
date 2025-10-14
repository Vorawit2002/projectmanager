using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.EmailLogs.Commands.CreateEmailLog;
public class CreateEmailLogCommand : IRequest<bool>
{
    public string Subject { get; set; } = default!;
    public string SentTo { get; set; } = default!;
    public string Massage { get; set; } = default!;
    public string SendBy { get; set; } = default!;
    public string? SendType { get; set; }
    public string? RefEntityId { get; set; }
    public string? RefEntityClass { get; set; }
    public bool SendStatus { get; set; }
}

public class CreateEmailLogCommandHandler : IRequestHandler<CreateEmailLogCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ISender _sender;
    public CreateEmailLogCommandHandler(IApplicationDbContext context, ISender sender)
    {
        _context = context;
        _sender = sender;

    }
    public async Task<bool> Handle(CreateEmailLogCommand request, CancellationToken cancellationToken)
    {
        var emailLog = new EmailLog();
        emailLog.Subject = request.Subject;
        emailLog.SentTo = request.SentTo;
        emailLog.Massage = request.Massage;
        emailLog.SendBy = request.SendBy;
        emailLog.SendType = request.SendType;
        emailLog.RefEntityId = request.RefEntityId;
        emailLog.RefEntityClass = request.RefEntityClass;
        emailLog.SendStatus = request.SendStatus;
        emailLog.SendDate = DateTime.Now;

        await _context.EmailLogs.AddAsync(emailLog);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
