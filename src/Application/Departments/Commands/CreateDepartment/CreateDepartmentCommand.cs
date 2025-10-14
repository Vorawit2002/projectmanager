using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Departments.Commands.CreateDepartment;
public class CreateDepartmentCommand : IRequest<bool>
{
    public string Name { get; set; } = default!;
    public bool IsActive { get; set; }


}
public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public CreateDepartmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var Departments = new Department();
        Departments.Name= request.Name;
        Departments.IsActive= request.IsActive;
        await _context.Departments.AddAsync(Departments);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
