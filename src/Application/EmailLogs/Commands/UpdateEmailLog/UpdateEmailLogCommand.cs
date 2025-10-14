using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;

namespace ProjectManagement.Application.EmailLogs.Commands.UpdateEmailLog;
public class UpdateEmailLogCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Subject { get; set; } = default!;
    public string SentTo { get; set; } = default!;
    public string Massage { get; set; } = default!;
    public string SendBy { get; set; } = default!;
    public string? SendType { get; set; }
    public string? RefEntityId { get; set; }
    public string? RefEntityClass { get; set; }
    public bool SendStatus { get; set; }
}

public class UpdateEmailLogCommandHandler : IRequestHandler<UpdateEmailLogCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ISender _sender;

    public UpdateEmailLogCommandHandler(IApplicationDbContext context, ISender sender)
    {
        _context = context;
        _sender = sender;
    }
    public async Task<bool> Handle(UpdateEmailLogCommand request, CancellationToken cancellationToken)
    {
        var emailLog = await _context.EmailLogs.FirstOrDefaultAsync(x => x.Id == request.Id);
        if (emailLog == null)
        {
            return false;
        }

        emailLog.Subject = request.Subject;
        emailLog.SentTo = request.SentTo;
        emailLog.Massage = request.Massage;
        emailLog.SendBy = request.SendBy;
        emailLog.SendType = request.SendType;
        emailLog.RefEntityId = request.RefEntityId;
        emailLog.RefEntityClass = request.RefEntityClass;
        emailLog.SendStatus = request.SendStatus;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
