using ProjectManagement.Application.Common.Models;

using Microsoft.AspNetCore.Http.HttpResults;
using ProjectManagement.Application.EventTypes.Command.UpdateEventType;
using ProjectManagement.Application.EventTypes.Command.CreateEventType;
using ProjectManagement.Application.EventTypes.Queries;


namespace ProjectManagement.Web.Endpoints;

public class EventType : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(CreateEventType, "CreateEventType")
            .MapGet(GetEventTypeQuery, "GetEventTypeQuery")
            .MapGet(GetEventTypeQueryByID, "GetEventTypeQueryByID/{id}")
            .MapPost(GetEventTypeWithPagination, "GetEventTypeWithPagination")
            .MapPut(UpdateEventType, "UpdateEventType")
            .MapDelete(DeleteEventType, "DeleteEventType/{id}");
    }

    public async Task<bool> CreateEventType(ISender sender, CreateEventTypeCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<PaginatedList<EventTypeDto>> GetEventTypeWithPagination(ISender sender, GetEventTypeWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
    public async Task<bool> UpdateEventType(ISender sender, UpdateEventTypeCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<IEnumerable<EventTypeDto>> GetEventTypeQuery(ISender sender)
    {
        GetEventTypeQuery query = new GetEventTypeQuery();
        return await sender.Send(query);
    }
    public async Task<EventTypeDto> GetEventTypeQueryByID(ISender sender, Guid id)
    {
        GetEventTypeByIdQuery query = new GetEventTypeByIdQuery(id);
        return await sender.Send(query);
    }
    public async Task<bool> DeleteEventType(ISender sender, Guid id)
    {
        DeleteEventTypeCommand query = new DeleteEventTypeCommand(id);
        return await sender.Send(query);
    }
}
