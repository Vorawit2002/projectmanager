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

namespace ProjectManagement.Application.CheckInCheckOuts.Commands.DeleteCheckInCheckOut;
public record DeteleCheckInCheckOutCommand(Guid Id) : IRequest<bool>;
public class DeteleCheckInCheckOutCommandHandler : IRequestHandler<DeteleCheckInCheckOutCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailSenderService _emailSenderService;
    private readonly ISender _sender;
    public DeteleCheckInCheckOutCommandHandler(IApplicationDbContext context, IEmailSenderService emailSenderService, ISender sender)
    {
        _context = context;
        _emailSenderService = emailSenderService;
        _sender = sender;
    }
    public async Task<bool> Handle(DeteleCheckInCheckOutCommand request, CancellationToken cancellationToken)
    {
        var Checkinout = await _context.CheckInCheckOuts.FirstOrDefaultAsync(x => x.Id == request.Id);
        Guard.Against.NotFound(request.Id, Checkinout);
        _context.CheckInCheckOuts.Remove(Checkinout);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
