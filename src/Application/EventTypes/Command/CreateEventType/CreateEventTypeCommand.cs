using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.ActivityPlanContacts.Command.CreateActivityPlanContact;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.EventTypes.Command.CreateEventType;
public class CreateEventTypeCommand : IRequest<bool>
{
    public string Name { get; set; } = default!;
}
public class CreateEventTypeCommandHandler : IRequestHandler<CreateEventTypeCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public CreateEventTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(CreateEventTypeCommand request, CancellationToken cancellationToken)
    {
        // หาตัวล่าสุดที่มีการสร้าง
        var latestEventType = await _context.EventTypes
            .OrderByDescending(x => x.Created)
            .FirstOrDefaultAsync(cancellationToken);


        // เริ่มต้นด้วย "001"
        string nextEventTypeCode = "001";

        if (latestEventType != null && !string.IsNullOrEmpty(latestEventType.EventTypeCode))
        {
            // แปลงรหัสล่าสุดเป็นเลข แล้ว +1
            if (int.TryParse(latestEventType.EventTypeCode, out int latestCodeNumber))
            {
                nextEventTypeCode = (latestCodeNumber + 1).ToString("D3"); // format เป็น 3 หลัก เช่น 002
            }
        }

        var eventTypes = new EventType();
        eventTypes.Name = request.Name;
        eventTypes.EventTypeCode = nextEventTypeCode;
       
        await _context.EventTypes.AddAsync(eventTypes);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
