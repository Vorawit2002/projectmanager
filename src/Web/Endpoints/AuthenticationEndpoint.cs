using ProjectManagement.Application.Authentication.Commands.Login;
using ProjectManagement.Application.Authentication.Commands.Register;
using ProjectManagement.Application.Authentication.Commands.RefreshToken;
using ProjectManagement.Application.Authentication.Commands.MigrateExistingUsers;
using ProjectManagement.Application.ApplicationUserProfile.Queries;
using ProjectManagement.Application.Authentication.Queries;
using ProjectManagement.Domain.Constants;

namespace ProjectManagement.Web.Endpoints;

public class AuthenticationEndpoint : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(Login, "login")
            .MapPost(Register, "register")
            .MapPost(RefreshToken, "refresh-token");
            
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetCurrentUser, "me");
            
        app.MapGroup(this)
            .RequireAuthorization(Roles.Administrator)
            .MapPost(MigrateExistingUsers, "migrate-users");
    }

    public async Task<LoginResponseDto> Login(ISender sender, LoginCommand command)
    {
        var result = await sender.Send(command);

        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessException(string.Join(", ", result.Errors));
        }

        return result.Data!;
    }

    public async Task<IResult> Register(ISender sender, RegisterCommand command)
    {
        var result = await sender.Send(command);

        if (!result.Succeeded)
        {
            return Results.BadRequest(new
            {
                errors = result.Errors
            });
        }

        return Results.Ok(new
        {
            message = "ลงทะเบียนสำเร็จ"
        });
    }

    public async Task<IResult> RefreshToken(ISender sender, RefreshTokenCommand command)
    {
        var result = await sender.Send(command);

        if (!result.Succeeded)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(result.Data);
    }

    public async Task<CurrentUserDto> GetCurrentUser(ISender sender)
    {
        var command = new GetApplicationUserProfileCommand();
        return await sender.Send(command);
    }

    public async Task<IResult> MigrateExistingUsers(ISender sender, MigrateExistingUsersCommand command)
    {
        var result = await sender.Send(command);

        if (!result.Succeeded)
        {
            return Results.BadRequest(new
            {
                errors = result.Errors
            });
        }

        return Results.Ok(result.Data);
    }
}
