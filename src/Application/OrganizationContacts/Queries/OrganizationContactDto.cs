using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.OrganizationContacts.Queries;
public class OrganizationContactDto
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public virtual Organization? Organizations { get; set; }
    public string? TitleName { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? Position { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? LineId { get; set; }
    public Guid? AttachmentId { get; set; }
    public string? ImageProfile { get; set; }
    public DateTime Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime LastModified { get; set; }
    public string? LastModifiedBy { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<OrganizationContact, OrganizationContactDto>();
        }
    }
}
