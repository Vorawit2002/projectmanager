using ProjectManagement.Application.Common.Models;

using Microsoft.AspNetCore.Http.HttpResults;
using ProjectManagement.Application.ProjectContacts.Queries;
using ProjectManagement.Application.Projects.Command.CreateProjectContact;
using ProjectManagement.Application.Projects.Command.UpdateProjectContact;
using ProjectManagement.Application.Projects.Command.DeleteProjectContact;


namespace ProjectManagement.Web.Endpoints;

public class ProjectContacts : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(CreateProjectContact, "CreateProjectContact")
            .MapGet(GetProjectContactQuery, "GetProjectContactQuery")
            .MapGet(GetProjectContactQueryByID, "GetProjectContactQueryByID")
            .MapPost(GetProjectContactWithPagination, "GetProjectContactWithPagination")
            .MapPut(UpdateProjectContact, "UpdateProjectContact")
            .MapDelete(DeleteProjectContact, "DeleteProjectContact/{id}");
    }

    public async Task<bool> CreateProjectContact(ISender sender, CreateProjectContactCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<PaginatedList<ProjectContactDto>> GetProjectContactWithPagination(ISender sender, GetProjectContactWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
    public async Task<bool> UpdateProjectContact(ISender sender, UpdateProjectContactCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<IEnumerable<ProjectContactDto>> GetProjectContactQuery(ISender sender)
    {
        GetProjectContactQuery query = new GetProjectContactQuery();
        return await sender.Send(query);
    }
    public async Task<ProjectContactDto> GetProjectContactQueryByID(ISender sender, Guid id)
    {
        GetProjectContactByIdQuery query = new GetProjectContactByIdQuery(id);
        return await sender.Send(query);
    }
    public async Task<bool> DeleteProjectContact(ISender sender, Guid id)
    {
        DeleteProjectContactCommand query = new DeleteProjectContactCommand(id);
        return await sender.Send(query);
    }
}
