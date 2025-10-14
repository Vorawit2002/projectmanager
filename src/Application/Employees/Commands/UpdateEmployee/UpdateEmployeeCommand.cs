using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Employees.Commands.UpdateEmployee;
public class UpdateEmployeeCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string? TitleName { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Position { get; set; } = default!;
    public string? Phone { get; set; }
    public Guid DepartmentId { get; set; }
    public string? ImageProfile { get; set; }
}
public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateEmployeeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employees = await _context.Employees.FirstOrDefaultAsync(x => x.Id == request.Id);
        Guard.Against.NotFound(request.Id, employees);
        employees.TitleName = request.TitleName;
        employees.FirstName = request.FirstName;
        employees.LastName = request.LastName;
        employees.Email = request.Email;
        employees.Position = request.Position;
        employees.Phone = request.Phone;
        employees.DepartmentId = request.DepartmentId;
        employees.ImageProfile = request.ImageProfile;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
