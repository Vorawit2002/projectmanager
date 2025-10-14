using Microsoft.AspNetCore.Http.HttpResults;
using ProjectManagement.Application.ActivityPlans.Commands.CreateActivityPlan;
using ProjectManagement.Application.ActivityPlans.Commands.DeleteActivityPlan;
using ProjectManagement.Application.ActivityPlans.Commands.ShareActivityPlan;
using ProjectManagement.Application.ActivityPlans.Commands.UpdateActivityPlan;
using ProjectManagement.Application.ActivityPlans.Queries;
using ProjectManagement.Application.Common.Models;

namespace ProjectManagement.Web.Endpoints;

public class ActivityPlanEndpoint : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(ShareActivityPlan, "ShareActivityPlan")
            .MapPost(CreateActivityPlan, "CreateActivityPlan")
            .MapPost(DuplicateActivityPlan, "DuplicateActivityPlan")
            .MapGet(GetActivityPlanQuery, "GetActivityPlanQuery")
            .MapGet(GetActivityPlanQueryByID, "GetActivityPlanQueryByID/{id}")
            .MapGet(GetActivityPlanWithPlanNoteForExcel, "GetActivityPlanWithPlanNoteForExcel")
            .MapGet(GetActivityPlanQueryWithYears, "GetActivityPlanQueryWithYears")
            .MapPost(GetNotificationActivityPlanQueryByEmployeeId, "GetNotificationActivityPlanQueryByEmployeeId")
            .MapPost(GetActivityPlanForDashboardQuery, "GetActivityPlanForDashboardQuery")
            .MapPost(GetActivityPlanQueryLatest, "GetActivityPlanQueryLatest")
            .MapPost(GetActivityPlanWithPagination, "GetActivityPlanWithPagination")
            .MapPost(GetActivityPlanWithPlanNoteWithPagination, "GetActivityPlanWithPlanNoteWithPaginationQuery")
            .MapPost(GetActivityPlanQueryByEmployeeId, "GetActivityPlanQueryByEmployeeId")
            .MapPut(UpdateActivityPlan, "UpdateActivityPlan")
            .MapDelete(DeleteActivityPlan, "DeleteActivityPlan/{id}");
    }
    public async Task<bool> ShareActivityPlan(ISender sender, ShareActivityPlanCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<Guid> CreateActivityPlan(ISender sender, CreateActivityPlanCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<bool> DuplicateActivityPlan(ISender sender, DuplicateActivityPlanCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<DashboardActivityPlanDto> GetActivityPlanForDashboardQuery(ISender sender, GetActivityPlanQueryForDashboard command)
    {
        return await sender.Send(command);
    }
    public async Task<Guid?> GetActivityPlanQueryLatest(ISender sender, GetActivityPlanLatestQuery command)
    {
        return await sender.Send(command);
    }
    public async Task<ActivityPlanYearsDto> GetActivityPlanQueryWithYears(ISender sender)
    {
        GetActivityPlanWithYearsQuery query = new GetActivityPlanWithYearsQuery();
        return await sender.Send(query);
    }
    public async Task<PaginatedListForActivity<ActivityPlanDto>> GetActivityPlanWithPagination(ISender sender, GetActivityPlanWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
    public async Task<PaginatedList<ActivityPlanExcelDto>> GetActivityPlanWithPlanNoteWithPagination(ISender sender, GetActivityPlanWithPlanNoteWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
    public async Task<bool> UpdateActivityPlan(ISender sender, UpdateActivityPlanCommand query)
    {
        return await sender.Send(query);
    }
    public async Task<IEnumerable<ActivityPlanDto>> GetActivityPlanQuery(ISender sender)
    {
        GetActivityPlanQuery query = new GetActivityPlanQuery();
        return await sender.Send(query);
    }
    public async Task<IEnumerable<ActivityPlanNotificationDto>> GetNotificationActivityPlanQueryByEmployeeId(ISender sender, GetNotificationActivityPlanByEmployeeIdQuery command)
    {
        return await sender.Send(command);
    }
    public async Task<IEnumerable<ActivityPlanExcelDto>> GetActivityPlanWithPlanNoteForExcel(ISender sender)
    {
        GetActivityPlanWithPlanNoteQueryForExcel query = new GetActivityPlanWithPlanNoteQueryForExcel();
        return await sender.Send(query);
    }
    public async Task<IEnumerable<ActivityPlanDto>> GetActivityPlanQueryByEmployeeId(ISender sender, GetActivityPlanByEmployeeIdQuery query)
    {
        return await sender.Send(query);
    }
    public async Task<ActivityPlanDto> GetActivityPlanQueryByID(ISender sender, Guid id)
    {
        GetActivityPlanByIdQuery query = new GetActivityPlanByIdQuery(id);
        return await sender.Send(query);
    }
    public async Task<bool> DeleteActivityPlan(ISender sender, Guid Id)
    {
        DeleteActivityPlanCommand command = new DeleteActivityPlanCommand(Id);
        return await sender.Send(command);
    }

}
