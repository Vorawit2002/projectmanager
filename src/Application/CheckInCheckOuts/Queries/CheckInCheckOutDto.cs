using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Departments.Queries;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.CheckInCheckOuts.Queries;
public class CheckInCheckOutDto
{
    public Guid Id { get; set; }
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
    public string? CheckInImage { get; set; }
    public string? CheckOutImage { get; set; }
    public CheckInCheckOutType? CheckInCheckOutTypes { get; set; }
    public string? Types { get; set; }
    public DateTime Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CheckInCheckOut, CheckInCheckOutDto>();
        }
    }
}
