using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Projects.Command.UpdateProject;
public class UpdateProjectCommand : IRequest<bool>
{
    public Guid Id { get; set; } 
    public string ProjectCode { get; set; } = default!;
    public string ProjectName { get; set; } = default!;
    public string ContractNumber { get; set; } = default!;
    public DateTime? ContractSignedDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? WarrantyEndDate { get; set; }
    public Guid OrganizationId { get; set; }
    public decimal ProjectCost { get; set; }
    public ProjectType ProjectType { get; set; }
    public string? ShortName { get; set; }
}
public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateProjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == request.Id);
        Guard.Against.NotFound(request.Id, project);
        project.ProjectCode = request.ProjectCode;
        project.ProjectName = request.ProjectName;
        project.ContractNumber = request.ContractNumber;
        if (request.ContractSignedDate.HasValue)
        {
            project.ContractSignedDate = request.ContractSignedDate.Value.ToLocalTime();
        }
        else {
            project.ContractSignedDate = null;
        }
        if (request.StartDate.HasValue)
        {
            project.StartDate = request.StartDate.Value.ToLocalTime();
        }
        else
        {
            project.StartDate = null;
        }
        if (request.EndDate.HasValue)
        {
            project.EndDate = request.EndDate.Value.ToLocalTime();
        }
        else
        {
            project.EndDate = null;
        }
        if (request.WarrantyEndDate.HasValue)
        {
            project.WarrantyEndDate = request.WarrantyEndDate.Value.ToLocalTime();
        }
        else
        {
            project.WarrantyEndDate = null;
        }
        project.OrganizationId = request.OrganizationId;
        project.ProjectCost = request.ProjectCost;
        project.ProjectType = request.ProjectType;
        project.ShortName = request.ShortName;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
