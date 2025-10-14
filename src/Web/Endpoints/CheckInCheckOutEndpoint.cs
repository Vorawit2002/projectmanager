using Microsoft.AspNetCore.Http.HttpResults;
using ProjectManagement.Application.CheckInCheckOuts.Commands.CreateCheckInCheckOut;
using ProjectManagement.Application.CheckInCheckOuts.Commands.DeleteCheckInCheckOut;
using ProjectManagement.Application.CheckInCheckOuts.Commands.UpdateCheckInCheckOut;
using ProjectManagement.Application.CheckInCheckOuts.Queries;
using ProjectManagement.Application.Common.Models;

namespace projectmanagement.Web.Endpoints;

public class CheckInCheckOutEndpoint : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CheckLineUserIdCheckInCheckOutToDay, "CheckLineUserIdCheckInCheckOutToDay")
             .MapPost(CreateCheckInCheckOutLine, "CreateCheckInCheckOutLine");
        app.MapGroup(this)
        .RequireAuthorization()
        .MapPost(CreateCheckInCheckOut, "CreateCheckInCheckOut")
        .MapPost(GetCheckInCheckOutQueryWithPagination, "GetCheckInCheckOutQueryWithPagination")
        .MapPost(CheckEmployeeCheckInCheckOutQuery, "CheckEmployeeCheckInCheckOutQuery")
        .MapGet(GetCheckInCheckOutQuery, "GetCheckInCheckOutQuery")
        .MapGet(GetCheckInCheckOutQueryById, "GetCheckInCheckOutQueryById/{Id}")
        .MapGet(GetCheckInCheckOutQueryByEmployeeId, "GetCheckInCheckOutQueryByEmployeeId/{Id}")
        .MapPut(UpdateCheckInCheckOut, "UpdateCheckInCheckOut")
        .MapDelete(DeleteCheckInCheckOut, "DeleteCheckInCheckOut/{Id}");
    }
    public async Task<CreateCheckInCheckOutDto> CreateCheckInCheckOut(ISender sender, CreateCheckInCheckOutCommand query)
    {
        return await sender.Send(query);
    }
    public async Task<CheckLineUserIdCheckInCheckOutToDayDto> CheckLineUserIdCheckInCheckOutToDay(ISender sender, CheckLineUserIdCheckInCheckOutToDayQuery query)
    {
        return await sender.Send(query);
    }
    public async Task<CreateCheckInCheckOutDto> CreateCheckInCheckOutLine(ISender sender, CreateCheckInCheckOutForLineCommand query)
    {
        return await sender.Send(query);
    }
    public async Task<IEnumerable<CheckInCheckOutDto>> GetCheckInCheckOutQuery(ISender sender)
    {
        GetCheckInCheckOutQuery command = new GetCheckInCheckOutQuery();
        return await sender.Send(command);
    }
    public async Task<CheckEmployeeCheckInCheckOutDto> CheckEmployeeCheckInCheckOutQuery(ISender sender, CheckEmployeeCheckInCheckOut command)
    {
        return await sender.Send(command);
    }
    public async Task<CheckInCheckOutDto> GetCheckInCheckOutQueryById(ISender sender, Guid Id)
    {
        GetCheckInCheckOutByIdQuery command = new GetCheckInCheckOutByIdQuery(Id);
        return await sender.Send(command);
    }
    public async Task<IList<CheckInCheckOutDto>> GetCheckInCheckOutQueryByEmployeeId(ISender sender, Guid Id)
    {
        GetCheckInCheckOutByEmployeeIdQuery command = new GetCheckInCheckOutByEmployeeIdQuery(Id);
        return await sender.Send(command);
    }
    public async Task<PaginatedList<CheckInCheckOutDto>> GetCheckInCheckOutQueryWithPagination(ISender sender, GetCheckInCheckOutWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
    public async Task<bool> UpdateCheckInCheckOut(ISender sender, UpdateCheckInCheckOutCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<bool> DeleteCheckInCheckOut(ISender sender, Guid Id)
    {
        DeteleCheckInCheckOutCommand command = new DeteleCheckInCheckOutCommand(Id);
        return await sender.Send(command);
    }
}
