using ProjectManagement.Application.Common.Models;

using Microsoft.AspNetCore.Http.HttpResults;
using ProjectManagement.Application.ActivityPlanContacts.Command.CreateActivityPlanContact;
using ProjectManagement.Application.ActivityPlanContacts.Queries;
using ProjectManagement.Application.ActivityPlans.Commands.CreateActivityPlan;
using ProjectManagement.Application.ActivityPlanContacts.Command.UpdateActivityPlanContact;
using ProjectManagement.Application.ActivityPlanContacts.Command.DeleteActivityPlanContact;

namespace ProjectManagement.Web.Endpoints;

public class ActivityPlanContacts : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(CreateActivityPlanContact, "CreateActivityPlanContact")
            .MapGet(GetActivityPlanContactQuery, "GetActivityPlanContactQuery")
            .MapGet(GetActivityPlanContactQueryByID, "GetActivityPlanContactQueryByID/{id}")
            .MapPost(GetActivityPlanContactWithPagination, "GetActivityPlanContactWithPagination")
            .MapPost(GetActivityPlanContactQueryByActivityPlanId, "GetActivityPlanContactQueryByActivityPlanId")
            .MapPut(UpdateActivityPlanContact, "UpdateActivityPlanContact")
            .MapDelete(DeleteActivityPlanContact, "DeleteActivityPlanContact/{id}");
    }

    public async Task<bool> CreateActivityPlanContact(ISender sender, CreateActivityPlanContactCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<PaginatedList<ActivityPlanContactDto>> GetActivityPlanContactWithPagination(ISender sender, GetActivityPlanContactWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
    public async Task<bool> UpdateActivityPlanContact(ISender sender, UpdateActivityPlanContactCommand query)
    {
        return await sender.Send(query);
    }
    public async Task<IEnumerable<ActivityPlanContactDto>> GetActivityPlanContactQueryByActivityPlanId(ISender sender, GetActivityPlanContactByActivityPlanIdQuery query)
    {
        return await sender.Send(query);
    }
    public async Task<IEnumerable<ActivityPlanContactDto>> GetActivityPlanContactQuery(ISender sender)
    {
        GetActivityPlanContactQuery query = new GetActivityPlanContactQuery();
        return await sender.Send(query);
    }
    public async Task<ActivityPlanContactDto> GetActivityPlanContactQueryByID(ISender sender, Guid id)
    {
        GetActivityPlanContactByIdQuery query = new GetActivityPlanContactByIdQuery(id);
        return await sender.Send(query);
    }
    public async Task<bool> DeleteActivityPlanContact(ISender sender, Guid Id)
    {
        DeleteActivityPlanContactCommand command = new DeleteActivityPlanContactCommand(Id);
        return await sender.Send(command);
    }
}
