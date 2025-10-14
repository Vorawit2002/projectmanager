using ProjectManagement.Application.Common.Models;

using Microsoft.AspNetCore.Http.HttpResults;
using ProjectManagement.Application.OrganizationContacts.Queries;
using ProjectManagement.Application.OrganizationContacts.Commands.CreateOrganizationContact;
using ProjectManagement.Application.OrganizationContacts.Commands.UpdateOrganizationContact;
using ProjectManagement.Application.OrganizationContacts.Commands.DeleteOrganizationContact;


namespace ProjectManagement.Web.Endpoints;

public class OrganizationContacts : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(CreateOrganizationContact, "CreateOrganizationContact")
            .MapPost(CreateOrganizationContactJustName, "CreateOrganizationContactJustName")
            .MapGet(GetOrganizationContactQuery, "GetOrganizationContactQuery")
            .MapGet(GetOrganizationContactQueryByID, "GetOrganizationContactQueryByID/{id}")
            .MapPost(GetOrganizationContactWithPagination, "GetOrganizationContactWithPagination")
            .MapPost(GetOrganizationContactQueryByOrganizationId, "GetOrganizationContactQueryByOrganizationId")
            .MapPut(UpdateOrganizationContact, "UpdateOrganizationContact")
            .MapDelete(DeleteOrganizationContact, "DeleteOrganizationContact/{id}");
    }

    public async Task<bool> CreateOrganizationContact(ISender sender, CreateOrganizationContactCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<Guid> CreateOrganizationContactJustName(ISender sender, CreateOrganizationContactJustNameCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<PaginatedList<OrganizationContactDto>> GetOrganizationContactWithPagination(ISender sender, GetOrganizationContactWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
    public async Task<bool> UpdateOrganizationContact(ISender sender, UpdateOrganizationContactCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<IEnumerable<OrganizationContactDto>> GetOrganizationContactQueryByOrganizationId(ISender sender, GetOrganizationContactByOrganizationIdQuery query)
    {
        return await sender.Send(query);
    }
    public async Task<IEnumerable<OrganizationContactDto>> GetOrganizationContactQuery(ISender sender)
    {
        GetOrganizationContactQuery query = new GetOrganizationContactQuery();
        return await sender.Send(query);
    }
    public async Task<OrganizationContactDto> GetOrganizationContactQueryByID(ISender sender, Guid id)
    {
        GetOrganizationContactByIdQuery query = new GetOrganizationContactByIdQuery(id);
        return await sender.Send(query);
    }
    public async Task<bool> DeleteOrganizationContact(ISender sender, Guid id)
    {
        DeleteOrganizationContactCommand query = new DeleteOrganizationContactCommand(id);
        return await sender.Send(query);
    }
}
