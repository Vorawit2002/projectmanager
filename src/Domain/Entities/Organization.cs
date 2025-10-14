using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Entities;
public class Organization:BaseAuditableEntity
{
    public string Name { get; set; } = default!;
    public string? ShortName { get; set; } 
    public TypeOrganization? TypeOrganization { get; set; }
    public string? Address { get; set; } 
    public string? Coordinates { get; set; }
    public string? WebSite { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public IList<OrganizationContact> OrganizationContacts { get; set; } = new List<OrganizationContact>();
    //public IList<Project> Projects { get; set; } = new List<Project>();
}
