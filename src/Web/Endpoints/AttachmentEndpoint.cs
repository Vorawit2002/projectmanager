using ProjectManagement.Application.ActivityPlans.Queries;
using ProjectManagement.Application.ApplicationUserProfile.Queries;
using ProjectManagement.Application.Attachments.Queries;
using ProjectManagement.Application.Authentication.Queries;

namespace projectmanagement.Web.Endpoints;

public class AttachmentEndpoint:EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
        .RequireAuthorization()
        .MapGet(GetAttachmentQueryById, "GetAttachmentQueryById");
    }
    public async Task<AttachmentDto> GetAttachmentQueryById(ISender sender, Guid id)
    {
        GetAttachmentByIdQuery query = new GetAttachmentByIdQuery(id);
        return await sender.Send(query);
    }
}
