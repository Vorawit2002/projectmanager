using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Employees.Commands.CreateEmployee;
public class CheckAndCreateEmployeeCommand : IRequest<bool>
{
    public string UserId { get; set; } = default!;
    public string? TitleName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Email { get; set; } = default!;
    public string? Phone { get; set; }
    public string? Departments { get; set; }
    public string? Position { get; set; }
    public string? ImageProfile { get; set; }
    public string? Roles { get; set; }
    public string? Group { get; set; }
}
public class CheckAndCreateEmployeeCommandHandler : IRequestHandler<CheckAndCreateEmployeeCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public CheckAndCreateEmployeeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(CheckAndCreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Employees.FirstOrDefaultAsync(x => x.UserId == request.UserId);

        if (user == null)
        {
            var employees = new Employee();
            employees.UserId = request.UserId;
            employees.Email = request.Email;
            employees.Phone = request.Phone;
            employees.TitleName = request.TitleName;
            employees.FirstName = request.FirstName;
            employees.LastName = request.LastName;
            employees.ImageProfile = request.ImageProfile;
            employees.isActive = true;
            employees.Roles = request.Roles;
            employees.Group = request.Group;
            if (request.Departments != null && request.Departments != string.Empty)
            {
                var departments = await _context.Departments.FirstOrDefaultAsync(x => x.Name == request.Departments);
                if (departments == null)
                {
                    var Departments = new Department();
                    Departments.Name = request.Departments;
                    Departments.IsActive = true;
                    await _context.Departments.AddAsync(Departments);
                    employees.DepartmentId = Departments.Id;
                    await _context.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    employees.DepartmentId = departments.Id;
                }
            }
            employees.Position = request.Position;
            await _context.Employees.AddAsync(employees);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        else
        {
            user.Email = request.Email;
            user.Phone = request.Phone;
            user.TitleName = request.TitleName;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.ImageProfile = request.ImageProfile;
            user.Roles = request.Roles;
            user.Group = request.Group;
            if (request.Departments != null && request.Departments != string.Empty)
            {
                var departments = await _context.Departments.FirstOrDefaultAsync(x => x.Name == request.Departments);
                if (departments == null)
                {
                    var Departments = new Department();
                    Departments.Name = request.Departments;
                    Departments.IsActive = true;
                    await _context.Departments.AddAsync(Departments);
                    user.DepartmentId = Departments.Id;
                    await _context.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    user.DepartmentId = departments.Id;
                }
            }
            user.Position = request.Position;
            await _context.SaveChangesAsync(cancellationToken);
            return false;
        }
    }
}
