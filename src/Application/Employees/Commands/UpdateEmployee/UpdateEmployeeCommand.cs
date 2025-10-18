using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Exceptions;
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
    private readonly IUser _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IDataFilterService _dataFilterService;

    public UpdateEmployeeCommandHandler(
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
    
    public async Task<bool> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employees = await _context.Employees.FirstOrDefaultAsync(x => x.Id == request.Id);
        Guard.Against.NotFound(request.Id, employees);
        
        // Check authorization - Admin and Manager can update employees in their department
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
