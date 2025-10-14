using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;

namespace ProjectManagement.Application.CheckInCheckOuts.Queries;
public class CheckEmployeeCheckInCheckOut : IRequest<CheckEmployeeCheckInCheckOutDto>
{
    public Guid EmployeeId { get; set; }
    public DateTime Date { get; set; }
}
public class CheckEmployeeCheckInCheckOutHandler : IRequestHandler<CheckEmployeeCheckInCheckOut, CheckEmployeeCheckInCheckOutDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMinIOService _minIOService;
    public CheckEmployeeCheckInCheckOutHandler(IApplicationDbContext context, IMapper mapper, IMinIOService minIOService)
    {
        _context = context;
        _mapper = mapper;
        _minIOService = minIOService;
    }
    public async Task<CheckEmployeeCheckInCheckOutDto> Handle(CheckEmployeeCheckInCheckOut request, CancellationToken cancellationToken)
    {
        var checkInCheckOutDtos = await _context.CheckInCheckOuts
            .Include(x => x.Projects)
            .Include(x => x.Organizations)
            .Include(x => x.Employees)
            .FirstOrDefaultAsync(l => l.EmployeeId == request.EmployeeId && l.CheckIn.Date == request.Date.Date.ToLocalTime() && l.CheckOut == null);
        var dtos = new CheckEmployeeCheckInCheckOutDto();
        if (checkInCheckOutDtos == null)
        {
            dtos.Status = false;
            dtos.CurrentDate = DateTime.Now;
        }
        else
        {
            dtos.Status = true;
            dtos.CurrentDate = DateTime.Now;
            dtos.Id = checkInCheckOutDtos.Id;
        }
        return dtos;
    }
}
