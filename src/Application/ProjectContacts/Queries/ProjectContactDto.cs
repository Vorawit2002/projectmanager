using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.OrganizationContacts.Queries;
using ProjectManagement.Application.Projects.Queries;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.ProjectContacts.Queries;
public class ProjectContactDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public virtual Project? Projects { get; set; }
    public Guid OrganizationContactId { get; set; }
    public virtual OrganizationContact? OrganizationContacts { get; set; }
    public DateTime Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime LastModified { get; set; }
    public string? LastModifiedBy { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ProjectContact, ProjectContactDto>();
        }
    }
}
