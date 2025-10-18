using ProjectManagement.Application.Common.Models;

using Microsoft.AspNetCore.Http.HttpResults;
using ProjectManagement.Application.Departments.Queries;
using ProjectManagement.Application.Departments.Commands.CreateDepartment;
using ProjectManagement.Application.ActivityPlanContacts.Command.CreateActivityPlanContact;
using ProjectManagement.Application.Departments.Commands.UpdateDepartment;
using ProjectManagement.Application.Departments.Commands.DeleteDepartment;
using ProjectManagement.Domain.Constants;

namespace ProjectManagement.Web.Endpoints;

public class Departments : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(Policies.CanViewMasterData)
            .MapGet(GetDepartmentQuery, "GetDepartmentQuery")
            .MapGet(GetDepartmentQueryByID, "GetDepartmentQueryByID/{id}")
            .MapPost(GetDepartmentWithPagination, "GetDepartmentWithPagination");
            
        app.MapGroup(this)
            .RequireAuthorization(Policies.CanManageMasterData)
            .MapPost(CreateDepartment, "CreateDepartment")
            .MapPut(UpdateDepartment, "UpdateDepartment")
            .MapDelete(DeleteDepartment, "DeleteDepartment/{id}");
    }

    public async Task<bool> CreateDepartment(ISender sender, CreateDepartmentCommand command)
    {
        return await sender.Send(command);
    }

    public async Task<PaginatedList<DepartmentDto>> GetDepartmentWithPagination(ISender sender, GetDepartmentWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
    public async Task<bool> UpdateDepartment(ISender sender, UpdateDepartmentCommand query)
    {
        return await sender.Send(query);
    }
    public async Task<IEnumerable<DepartmentDto>> GetDepartmentQuery(ISender sender)
    {
        GetDepartmentQuery query = new GetDepartmentQuery();
        return await sender.Send(query);
    }
    public async Task<DepartmentDto> GetDepartmentQueryByID(ISender sender, Guid id)
    {
        GetDepartmentByIdQuery query = new GetDepartmentByIdQuery(id);
        return await sender.Send(query);
    }
    public async Task<bool> DeleteDepartment(ISender sender, Guid Id)
    {
        DeleteDepartmentCommand command = new DeleteDepartmentCommand(Id);
        return await sender.Send(command);
    }
}
