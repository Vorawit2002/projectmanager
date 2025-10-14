using ProjectManagement.Application.Common.Models;

using Microsoft.AspNetCore.Http.HttpResults;
using ProjectManagement.Application.PlanNotes.Command.CreatePlanNote;
using ProjectManagement.Application.PlanNotes.Command.DeletePlanNote;
using ProjectManagement.Application.PlanNotes.Queries;
using ProjectManagement.Application.Organizations.Queries;
using ProjectManagement.Application.PlanNotes.Command.UpdatePlanNote;


namespace ProjectManagement.Web.Endpoints;

public class PlanNotes : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(CreatePlanNote, "CreatePlanNote")
            .MapGet(GetPlanNoteQuery, "GetPlanNoteQuery")
            .MapGet(GetPlanNoteQueryByID, "GetPlanNoteQueryByID")
            .MapGet(GetPlanNoteQueryByActivityPlanId, "GetPlanNoteQueryByActivityPlanId")
            .MapPost(GetPlanNoteWithPagination, "GetPlanNoteWithPagination")
            .MapPut(UpdatePlanNote, "UpdatePlanNote")
            .MapDelete(DeletePlanNote, "DeletePlanNote/{id}");
    }

    public async Task<bool> CreatePlanNote(ISender sender, CreatePlanNoteCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<PaginatedList<PlanNoteDto>> GetPlanNoteWithPagination(ISender sender, GetPlannteWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
    public async Task<bool> UpdatePlanNote(ISender sender, UpdatePlanNoteCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<IEnumerable<PlanNoteDto>> GetPlanNoteQuery(ISender sender)
    {
        GetPlanNoteQuery query = new GetPlanNoteQuery();
        return await sender.Send(query);
    }
    public async Task<PlanNoteDto> GetPlanNoteQueryByID(ISender sender, Guid id)
    {
        GetPlanNoteByIdQuery query = new GetPlanNoteByIdQuery(id);
        return await sender.Send(query);
    }
    public async Task<PlanNoteDto> GetPlanNoteQueryByActivityPlanId(ISender sender, Guid id)
    {
        GetPlanNoteByActivityPlanIdQuery query = new GetPlanNoteByActivityPlanIdQuery(id);
        return await sender.Send(query);
    }
    public async Task<bool> DeletePlanNote(ISender sender, Guid id)
    {
        DeletePlanNoteCommand query = new DeletePlanNoteCommand(id);
        return await sender.Send(query);
    }
}
