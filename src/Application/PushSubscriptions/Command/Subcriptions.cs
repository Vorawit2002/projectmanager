using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Projects.Command.CreateProject;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.PushSubscriptions.Command;
public class SubcriptionsCommand : IRequest<bool>
{
    public Guid EmployeeId { get; set; }
    public string Endpoint { get; set; } = default!;
    public string P256dh { get; set; } = default!;
    public string Auth { get; set; } = default!;
}
public class SubcriptionsCommandHandler : IRequestHandler<SubcriptionsCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public SubcriptionsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(SubcriptionsCommand request, CancellationToken cancellationToken)
    {
        var pushSubscription = new PushSubscription();
        var existing = await _context.PushSubscriptions
        .FirstOrDefaultAsync(x => x.EmployeeId == request.EmployeeId && x.Endpoint == request.Endpoint);

        if (existing != null)
        {
            existing.IsActive = true;
            existing.P256dh = request.P256dh;
            existing.Auth = request.Auth;
        }
        else
        {
            pushSubscription.EmployeeId = request.EmployeeId;
            pushSubscription.Endpoint = request.Endpoint;
            pushSubscription.P256dh = request.P256dh;
            pushSubscription.Auth = request.Auth;
            pushSubscription.IsActive = true;
            _context.PushSubscriptions.Add(pushSubscription);
        }

        // อัพเดท Employee.Subscription ด้วย (ถ้าต้องการ)
        var emp = await _context.Employees.FindAsync(request.EmployeeId);
        if (emp != null) emp.Subscription = true;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
