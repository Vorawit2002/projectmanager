using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Projects.Queries;
public class ProjectDto
{
    public Guid Id { get; set; }
    public string ProjectCode { get; set; } = default!;
    public string ProjectName { get; set; } = default!;
    public string? ShortName { get; set; }
    public string ContractNumber { get; set; } = default!;
    public DateTime? ContractSignedDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? WarrantyEndDate { get; set; }
    public Guid? OrganizationId { get; set; }
    public decimal ProjectCost { get; set; }
    public ProjectType? ProjectType { get; set; }
    public DateTime Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Project, ProjectDto>();
        }
    }
}
