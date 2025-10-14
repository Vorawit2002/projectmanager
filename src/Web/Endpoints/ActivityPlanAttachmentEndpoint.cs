using ProjectManagement.Application.ActivityPlanAttachments.Command.DeleteActivityPlanAttachment;
using ProjectManagement.Application.ActivityPlanAttachments.Commands.CreateActivityPlanAttachment;
using ProjectManagement.Application.ActivityPlanAttachments.Queries;
using ProjectManagement.Application.ActivityPlanContacts.Command.DeleteActivityPlanContact;


namespace projectmanagement.Web.Endpoints;

public class ActivityPlanAttachmentEndpoint : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
        .RequireAuthorization()
        .MapPost(CreateActivityPlanAttachment, "CreateActivityPlanAttachment")
        .MapPost(GetActivityPlanAttachmentsQueryByActivityPlanId, "GetActivityPlanAttachmentsQueryByActivityPlanId")
        .MapDelete(DeleteActivityPlanAttachment, "DeleteActivityPlanAttachment");
    }
    public async Task<bool> CreateActivityPlanAttachment(ISender sender, CreateActivityPlanAttachmentCommand query)
    {
        return await sender.Send(query);
    }
    public async Task<List<ActivityPlanAttachmentDto>> GetActivityPlanAttachmentsQueryByActivityPlanId(ISender sender, GetActivityPlanAttachmentsByActivityPlanIdQuery command)
    {
        return await sender.Send(command);
    }
    public async Task<bool> DeleteActivityPlanAttachment(ISender sender, Guid Id)
    {
        DeleteActivityPlanAttachmentCommand command = new DeleteActivityPlanAttachmentCommand(Id);
        return await sender.Send(command);
    }
}
