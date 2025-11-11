using ProjectManagement.Application.Common.Models;

using Microsoft.AspNetCore.Http.HttpResults;
using ProjectManagement.Application.Organizations.Queries;
using ProjectManagement.Application.Organizations.Commands.CreateOrganization;
using ProjectManagement.Application.Organizations.Commands.UpdateOrganization;
using ProjectManagement.Application.Organizations.Commands.DeleteOrganization;
using ProjectManagement.Domain.Constants;


namespace ProjectManagement.Web.Endpoints;

public class Organizations : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        // Read operations - all authenticated users (including User role)
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetOrganizationQuery, "GetOrganizationQuery")
            .MapGet(GetOrganizationQueryByID, "GetOrganizationQueryByID/{id}")
            .MapPost(GetOrganizationWithPagination, "GetOrganizationWithPagination");
            
        // Write operations - requires CanModifyData policy (Admin, Manager, User - excludes Viewer role)
        app.MapGroup(this)
            .RequireAuthorization(Policies.CanModifyData)
            .MapPost(CreateOrganization, "CreateOrganization")
            .MapPost(CreateOrganizationJustName, "CreateOrganizationJustName")
            .MapPut(UpdateOrganization, "UpdateOrganization")
            .MapDelete(DeleteOrganization, "DeleteOrganization/{id}");
    }

    public async Task<Guid> CreateOrganization(ISender sender, CreateOrganizationCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<Guid> CreateOrganizationJustName(ISender sender, CreateOrganizationJustNameCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<PaginatedList<OrganizationDto>> GetOrganizationWithPagination(ISender sender, GetOrganizationWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
    public async Task<bool> UpdateOrganization(ISender sender, UpdateOrganizationCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<IEnumerable<OrganizationDto>> GetOrganizationQuery(ISender sender)
    {
        GetOrganizationQuery query = new GetOrganizationQuery();
        return await sender.Send(query);
    }
    public async Task<OrganizationDto> GetOrganizationQueryByID(ISender sender, Guid id)
    {
        GetOrganizationByIdQuery query = new GetOrganizationByIdQuery(id);
        return await sender.Send(query);
    }
    public async Task<bool> DeleteOrganization(ISender sender, Guid id)
    {
        DeleteOrganizationCommand query = new DeleteOrganizationCommand(id);
        return await sender.Send(query);
    }
}
