using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.EventTypes.Queries;
public class EventTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? EventTypeCode { get; set; }
    public DateTime Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<EventType, EventTypeDto>();
        }
    }
}
