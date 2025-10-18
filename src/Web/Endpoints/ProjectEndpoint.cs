using ProjectManagement.Application.Common.Models;

using Microsoft.AspNetCore.Http.HttpResults;
using ProjectManagement.Application.Projects.Command.DeleteProject;
using ProjectManagement.Application.Projects.Queries;
using ProjectManagement.Application.Projects.Command.UpdateProject;
using ProjectManagement.Application.Projects.Command.CreateProject;
using ProjectManagement.Domain.Constants;


namespace ProjectManagement.Web.Endpoints;

public class Projects : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        // Read operations - all authenticated users
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetProjectQuery, "GetProjectQuery")
            .MapGet(GetProjectQueryByID, "GetProjectQueryByID")
            .MapPost(GetProjectWithPagination, "GetProjectWithPagination");
            
        // Write operations - requires CanModifyData policy (excludes Viewer role)
        app.MapGroup(this)
            .RequireAuthorization(Policies.CanModifyData)
            .MapPost(CreateProject, "CreateProject")
            .MapPut(UpdateProject, "UpdateProject")
            .MapDelete(DeleteProject, "DeleteProject/{id}");
    }

    public async Task<bool> CreateProject(ISender sender, CreateProjectCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<PaginatedList<ProjectDto>> GetProjectWithPagination(ISender sender, GetProjectWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
    public async Task<bool> UpdateProject(ISender sender, UpdateProjectCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<IEnumerable<ProjectDto>> GetProjectQuery(ISender sender)
    {
        GetProjectQuery query = new GetProjectQuery();
        return await sender.Send(query);
    }
    public async Task<ProjectDto> GetProjectQueryByID(ISender sender, Guid id)
    {
        GetProjectByIdQuery query = new GetProjectByIdQuery(id);
        return await sender.Send(query);
    }
    public async Task<bool> DeleteProject(ISender sender, Guid id)
    {
        DeleteProjectCommand query = new DeleteProjectCommand(id);
        return await sender.Send(query);
    }
}
