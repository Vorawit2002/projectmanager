using ProjectManagement.Application.Common.Models;

using Microsoft.AspNetCore.Http.HttpResults;

using ProjectManagement.Application.Employees.Queries;
using ProjectManagement.Application.Employees.Commands.CreateEmployee;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using ProjectManagement.Application.Employees.Commands.UpdateEmployee;
using ProjectManagement.Application.Employees.Commands.DeleteEmployee;

namespace ProjectManagement.Web.Endpoints;

public class Employees : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
          //.RequireAuthorization()
          .MapPost(CheckAndCreateEmployee, "CheckAndCreateEmployee");
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(CreateEmployee, "CreateEmployee")
            //.MapPost(CheckAndCreateEmployee, "CheckAndCreateEmployee")
            .MapGet(GetEmployeeQuery, "GetEmployeeQuery")
            .MapGet(GetEmployeeQueryByID, "GetEmployeeQueryByID/{id}")
            .MapGet(GetEmployeeQueryByUserID, "GetEmployeeQueryByUserID/{UserId}")
            .MapPost(GetEmployeeWithPagination, "GetEmployeeWithPagination")
            .MapPost(GetEmployeeQueryByDepartmentId, "GetEmployeeQueryByDepartmentId")
            .MapPut(UpdateEmployee, "UpdateEmployee")
            .MapDelete(DeleteEmployee, "DeleteEmployee/{id}");
    }

    public async Task<bool> CreateEmployee(ISender sender, CreateEmployeeCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<bool> CheckAndCreateEmployee(ISender sender, CheckAndCreateEmployeeCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<PaginatedList<EmployeeDto>> GetEmployeeWithPagination(ISender sender, GetEmployeeWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
    public async Task<bool> UpdateEmployee(ISender sender, UpdateEmployeeCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<IEnumerable<EmployeeDto>> GetEmployeeQueryByDepartmentId(ISender sender, GetEmployeeByDepartmentIdQuery command)
    {
        return await sender.Send(command);
    }
    public async Task<IEnumerable<EmployeeDto>> GetEmployeeQuery(ISender sender)
    {
        GetEmployeeQuery query = new GetEmployeeQuery();
        return await sender.Send(query);
    }
    public async Task<EmployeeDto> GetEmployeeQueryByID(ISender sender, Guid id)
    {
        GetEmployeeByIdQuery query = new GetEmployeeByIdQuery(id);
        return await sender.Send(query);
    }
    public async Task<EmployeeDto> GetEmployeeQueryByUserID(ISender sender, string UserId)
    {
        GetEmployeeByUserIdQuery query = new GetEmployeeByUserIdQuery(UserId);
        return await sender.Send(query);
    }
    public async Task<bool> DeleteEmployee(ISender sender, Guid id)
    {
        DeleteEmployeeCommand query = new DeleteEmployeeCommand(id);
        return await sender.Send(query);
    }
}
