using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Exceptions;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Projects.Command.DeleteProject;
public record DeleteProjectCommand(Guid Id) : IRequest<bool>;
public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IDataFilterService _dataFilterService;

    public DeleteProjectCommandHandler(
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
    
    public async Task<bool> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == request.Id);
        Guard.Against.NotFound(request.Id, project);
        
        // Check authorization
        var userId = _currentUser.Id ?? throw new UnauthorizedAccessException();
        var roles = (await _identityService.GetUserRolesAsync(userId)).ToArray();
        
        var canAccess = await _dataFilterService.CanAccessResourceAsync(
            userId, 
            roles, 
            project.CreatedBy);
            
        if (!canAccess)
        {
            throw new ForbiddenAccessException();
        }
        
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
