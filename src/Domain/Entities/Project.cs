using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Entities;
public class Project : BaseAuditableEntity
{
    public string ProjectCode { get; set; } = default!;
    public string ProjectName { get; set; } = default!;
    public string? ShortName { get; set; }
    public string? ContractNumber { get; set; } 
    public DateTime? ContractSignedDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? WarrantyEndDate { get; set; }
    public Guid? OrganizationId { get; set; }
    public virtual Organization? Organizations { get; set; }
    public decimal? ProjectCost { get; set; }
    public ProjectType? ProjectType { get; set; }
    //public IList<ProjectContact> ProjectContacts { get; set; } = new List<ProjectContact>();
}
