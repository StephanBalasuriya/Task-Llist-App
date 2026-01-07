using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Dtos;
using WebApplication1.Models;


namespace WebApplication1.EndPoints;

public static class CreateDeleteEditGetTasks
{
    public static void MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/tasks");
        //Get all tasks
        group.MapGet("/", [Authorize] async (ClaimsPrincipal user, TaskStoreContext db) =>
        {
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var tasks = await db.Tasks
                .Where(t => t.UserId == userId)
                .Select(task => new TaskDetailsDto(
                    task.Id,
                    task.Title,
                    task.Description,
                    (TaskStatusDto)task.Status,
                    task.CreatedAt,
                    task.DueDate,
                    task.UpdatedAt
                ))
                .AsNoTracking()
                .ToListAsync();

            return Results.Ok(tasks);
        });
        // get task by taskid
        group.MapGet("/{taskId:int}", [Authorize] async (int taskId, ClaimsPrincipal user, TaskStoreContext db) =>
{
    var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    var task = await db.Tasks
        .Where(t => t.Id == taskId && t.UserId == userId)
        .Select(t => new TaskDetailsDto(
            t.Id,
            t.Title,
            t.Description,
            (TaskStatusDto)t.Status,
            t.CreatedAt,
            t.DueDate,
            t.UpdatedAt
        ))
        .AsNoTracking()
        .FirstOrDefaultAsync();

    return task is null ? Results.NotFound() : Results.Ok(task);
})
.WithName("GetTaskById");


        // Create a new task
        _ = group.MapPost("/post", [Authorize] async (ClaimsPrincipal user, TaskUpsertDto createTaskDto, TaskStoreContext db) =>
        {//validation input
            if (string.IsNullOrEmpty(createTaskDto.Title))
            {
                return Results.BadRequest("Task title cannot be empty.");
            }
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
  
            Tasks newTask = new()
            {
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                Status = (Models.TaskStatus)createTaskDto.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                DueDate = (DateTime)createTaskDto.DueDate,
                UserId = userId
            };
            db.Tasks.Add(newTask);
            await db.SaveChangesAsync();
            return Results.CreatedAtRoute(
                "GetTaskById",
                new {taskId = newTask.Id},
                new TaskDetailsDto(
                    newTask.Id,
                    newTask.Title,
                    newTask.Description,
                    (TaskStatusDto)newTask.Status,
                    newTask.CreatedAt,
                    newTask.DueDate,
                    newTask.UpdatedAt
                )
            );

        });

        //Update existing task
        group.MapPut("/update/{taskId:int}", [Authorize] async (int taskId, ClaimsPrincipal user, TaskUpsertDto updateTaskDto, TaskStoreContext db) =>
        {
            
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var existingTask = await db.Tasks
                .Where(t => t.Id == taskId && t.UserId == userId)
                .FirstOrDefaultAsync();

            if (existingTask is null)
            {
                return Results.NotFound();
            }

            existingTask.Title = updateTaskDto.Title;
            existingTask.Description = updateTaskDto.Description;
            existingTask.Status = (Models.TaskStatus)updateTaskDto.Status;
            existingTask.DueDate = (DateTime)updateTaskDto.DueDate;
            existingTask.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return Results.NoContent();
            
        });

        // Delete a task
        group.MapDelete("/delete/{taskId:int}", [Authorize] async (int taskId, ClaimsPrincipal user, TaskStoreContext db) =>
        {
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var existingTask = await db.Tasks
                .Where(t => t.Id == taskId && t.UserId == userId)
                .FirstOrDefaultAsync();

            if (existingTask is null)
            {
                return Results.NotFound();
            }

            db.Tasks.Remove(existingTask);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });








    }
}





//         //delete a game
//         group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
//         {

//             // var game = await dbContext.Games.FindAsync(id);
//             // if (game is not null)
//             // {
//             //     dbContext.Games.Remove(game);
//             //     await dbContext.SaveChangesAsync();
//             // }
//             // return Results.NoContent();


//             await dbContext.Games.Where(g => g.Id == id).ExecuteDeleteAsync();
//             return Results.NoContent();


//         });
//     }


// }
