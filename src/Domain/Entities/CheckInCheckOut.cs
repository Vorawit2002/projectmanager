using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Entities;
public class CheckInCheckOut: BaseAuditableEntity
{
    public Guid EmployeeId { get; set; }
    public virtual Employee? Employees { get; set; }
    public string? Location { get; set; }
    public string? LocationCheckOut { get; set; }
    public string? Lat { get; set; }
    public string? Long { get; set; }
    public string? IPAddress { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public Guid? OrganizationId { get; set; }
    public virtual Organization? Organizations { get; set; }
    public Guid? ProjectId { get; set; }
    public virtual Project? Projects { get; set; }
    public IList<CheckInCheckOutAttachment> CheckInCheckOutAttachments { get; set; } = new List<CheckInCheckOutAttachment>();
    public CheckInCheckOutType? CheckInCheckOutTypes { get; set; }
    public string? Types { get; set; }

}
