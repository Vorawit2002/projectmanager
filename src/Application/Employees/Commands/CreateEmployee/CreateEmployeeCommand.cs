using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Employees.Commands.CreateEmployee;
public class CreateEmployeeCommand : IRequest<bool>
{
    public string? TitleName { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Position { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string? ImageProfile { get; set; }
    public Guid DepartmentId { get; set; }
}
public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public CreateEmployeeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employees = new Employee();
        employees.FirstName = request.FirstName;
        employees.TitleName = request.TitleName;
        employees.LastName = request.LastName;
        employees.Position = request.Position;
        employees.Phone = request.Phone;
        employees.DepartmentId = request.DepartmentId;
        employees.ImageProfile = request.ImageProfile;
        await _context.Employees.AddAsync(employees);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
