using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Departments.Queries;
using ProjectManagement.Application.Employees.Queries;
using ProjectManagement.Application.Organizations.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.ActivityPlans.Queries;

public class GetActivityPlanByEmployeeIdQuery : IRequest<IEnumerable<ActivityPlanDto>>
{
    public List<Guid>? EmployeeId { get; set; }
    public string Years { get; set; } = default!;
    public Guid? DepartmentId  { get; set; }
    public List<Guid>? EventTypeId { get; set; }
}

public class GetActivityPlanByEmployeeIdQueryHandler : IRequestHandler<GetActivityPlanByEmployeeIdQuery, IEnumerable<ActivityPlanDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetActivityPlanByEmployeeIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<ActivityPlanDto>> Handle(GetActivityPlanByEmployeeIdQuery request, CancellationToken cancellationToken)
    {
        var QueryActivity = _context.ActivityPlans.AsQueryable();
        if (request.EventTypeId?.Any() == true)
        {
                QueryActivity = QueryActivity.Where(x => x.EventTypeId != null &&  request.EventTypeId.Contains((Guid)x.EventTypeId));
        }
        //กรองปี
        if (!string.IsNullOrEmpty(request.Years))
        {
            if (int.TryParse(request.Years, out int year))
            {
                QueryActivity = QueryActivity.Where(x => x.StartDate.Year == year && x.EndDate.Year == year);
            }
        }
        //กรองทั้งแผนก
        if (request.DepartmentId != null && request.DepartmentId != Guid.Empty) {
            var employeeIds = _context.Employees
       .Where(e => e.DepartmentId == request.DepartmentId)
       .Select(e => e.Id)
       .ToList();

            // แล้วกรอง activity ที่อยู่ใน employee กลุ่มนี้
            QueryActivity = QueryActivity.Where(x => employeeIds.Contains(x.EmployeeId));

        }
        //กรอง เฉพาะคน
        if (request.EmployeeId?.Any() == true) {
            QueryActivity = QueryActivity.Where(x => request.EmployeeId.Contains(x.EmployeeId));
        }
        return await QueryActivity
            .Include(x => x.Employees)
            .Include(x => x.Projects)
            .Include(x => x.Organizations)
           .ProjectTo<ActivityPlanDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}
