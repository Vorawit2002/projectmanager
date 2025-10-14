using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Projects.Command.DeleteProjectContact;
public record DeleteProjectContactCommand (Guid Id) : IRequest<bool>;
public class DeleteProjectContactCommandHandler : IRequestHandler<DeleteProjectContactCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteProjectContactCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(DeleteProjectContactCommand request, CancellationToken cancellationToken)
    {
        var projectContact = await _context.ProjectContacts.FirstOrDefaultAsync(x => x.Id == request.Id);
        Guard.Against.NotFound(request.Id, projectContact);
        _context.ProjectContacts.Remove(projectContact);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
