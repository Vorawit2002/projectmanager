using ProjectManagement.Application.PushSubscriptions.Command;

namespace projectmanagement.Web.Endpoints;

public class PushSubscriptionsEndpoint : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
        //.RequireAuthorization()
        .MapPost(Subcriptions, "Subcriptions")
        .MapPost(UnSubcriptions, "UnSubcriptions");
    }
    public async Task<bool> Subcriptions(ISender sender, SubcriptionsCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<bool> UnSubcriptions(ISender sender, UnSubcriptionsCommand command)
    {
        return await sender.Send(command);
    }
}
