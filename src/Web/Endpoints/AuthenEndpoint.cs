using ProjectManagement.Application.ApplicationUserProfile.Queries;
using ProjectManagement.Application.Authentication.Queries;

namespace projectmanagement.Web.Endpoints;

public class AuthenEndpoint:EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
        //.RequireAuthorization()
        .MapGet(GetUserProfile, "GetUserProfile");
    }
    public async Task<AuthenticationUserDto> GetUserProfile(ISender sender)
    {
        var command = new GetApplicationUserProfileCommand();
        return await sender.Send(command);
    }
}
