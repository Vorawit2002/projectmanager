using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Organizations.Queries;
public class OrganizationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? ShortName { get; set; }
    public TypeOrganization? TypeOrganization { get; set; }
    public string? Address { get; set; } 
    public string? Coordinates { get; set; }
    public string? WebSite { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public DateTime Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime LastModified { get; set; }
    public string? LastModifiedBy { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Organization, OrganizationDto>();
        }
    }
}
