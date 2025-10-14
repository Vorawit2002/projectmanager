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
public class UnSubcriptionsCommand : IRequest<bool>
{
    public Guid EmployeeId { get; set; }
    public string? Endpoint { get; set; }

}
public class UnSubcriptionsCommandHandler : IRequestHandler<UnSubcriptionsCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UnSubcriptionsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(UnSubcriptionsCommand request, CancellationToken cancellationToken)
    {
        var existing = await _context.PushSubscriptions
        .FirstOrDefaultAsync(x => x.EmployeeId == request.EmployeeId && x.Endpoint == request.Endpoint);

        if (existing != null)
            existing.IsActive = false;
       

        // อัพเดท Employee.Subscription ด้วย (ถ้าต้องการ)
        var emp = await _context.Employees.FindAsync(request.EmployeeId);
        if (emp != null) emp.Subscription = false;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
