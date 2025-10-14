using ProjectManagement.Application.TodoLists.Commands.CreateTodoList;
using ProjectManagement.Application.TodoLists.Commands.DeleteTodoList;
using ProjectManagement.Application.TodoLists.Commands.UpdateTodoList;
using ProjectManagement.Application.TodoLists.Queries.GetTodos;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ProjectManagement.Web.Endpoints;

public class TodoLists : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetTodoLists)
            .MapPost(CreateTodoList)
            .MapPut(UpdateTodoList, "{id}")
            .MapDelete(DeleteTodoList, "{id}");
    }

    public async Task<Ok<TodosVm>> GetTodoLists(ISender sender)
    {
        var vm = await sender.Send(new GetTodosQuery());

        return TypedResults.Ok(vm);
    }

    public async Task<Created<Guid>> CreateTodoList(ISender sender, CreateTodoListCommand command)
    {
        var id = await sender.Send(command);

        return TypedResults.Created($"/{nameof(TodoLists)}/{id}", id);
    }

    public async Task<Results<NoContent, BadRequest>> UpdateTodoList(ISender sender, Guid id, UpdateTodoListCommand command)
    {
        if (id != command.Id) return TypedResults.BadRequest();
        
        await sender.Send(command);

        return TypedResults.NoContent();
    }

    public async Task<NoContent> DeleteTodoList(ISender sender, Guid id)
    {
        await sender.Send(new DeleteTodoListCommand(id));

        return TypedResults.NoContent();
    }
}
