using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Exceptions;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Employees.Commands.DeleteEmployee;
public record DeleteEmployeeCommand(Guid Id) : IRequest<bool>;
public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IDataFilterService _dataFilterService;

    public DeleteEmployeeCommandHandler(
        IApplicationDbContext context,
        IUser currentUser,
        IIdentityService identityService,
        IDataFilterService dataFilterService)
    {
        _context = context;
        _currentUser = currentUser;
        _identityService = identityService;
        _dataFilterService = dataFilterService;
    }
    
    public async Task<bool> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employees = await _context.Employees.FirstOrDefaultAsync(x => x.Id == request.Id);
        Guard.Against.NotFound(request.Id, employees);
        
        // Check authorization - Admin and Manager can delete employees in their department
        var userId = _currentUser.Id ?? throw new UnauthorizedAccessException();
        var roles = (await _identityService.GetUserRolesAsync(userId)).ToArray();
        
        var canAccess = await _dataFilterService.CanAccessResourceAsync(
            userId, 
            roles, 
            employees.UserId,
            employees.DepartmentId);
            
        if (!canAccess)
        {
            throw new ForbiddenAccessException();
        }
        
        _context.Employees.Remove(employees);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
