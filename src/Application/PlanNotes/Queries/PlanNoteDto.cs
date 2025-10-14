using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.PlanNotes.Queries;
public class PlanNoteDto
{
    public Guid Id { get; set; }
    public Guid ActivityPlanId { get; set; }
    public virtual ActivityPlan? ActivityPlans { get; set; }
    public string Summary { get; set; } = default!;
    public string? ToDoNext { get; set; }
    public string? Remarks { get; set; }
    public DateTime Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<PlanNote, PlanNoteDto>();
        }
    }
}
