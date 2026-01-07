using System;
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
        group.MapGet("/{userId}", [Authorize] async (int userId, TaskStoreContext db) =>
        {
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
        group.MapGet("/{userId}/{id}", [Authorize] async (int userId, int id, TaskStoreContext db) =>
        {
            var task = await db.Tasks
                .Where(t => t.UserId == userId && t.Id == id)
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

            return task is null
        ? Results.NotFound("Task not found for this user")
        : Results.Ok(task);
        });

        // Create a new task
        _ = group.MapPost("/{userId}", [Authorize] async (int userId, TaskUpsertDto createTaskDto, TaskStoreContext db) =>
        {//validation input
            if (string.IsNullOrEmpty(createTaskDto.Title))
            {
                return Results.BadRequest("Task title cannot be empty.");
            }
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
                new { userId = newTask.UserId, id = newTask.Id },
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






    }
}

//         

//         //post new game
//         group.MapPost("/", async (CreateGameDto createGameDto, GameStoreContext dbcontext) =>
//         {

//             // // validating input
//             // if (string.IsNullOrEmpty(createGameDto.Name))
//             // {
//             //     return Results.BadRequest("Game name cannot be empty.");
//             // }//when using this method for validation, we have to type this for all inputs

//             // var newGame = new GameDto
//             // (
//             //     Id: games.Max(g => g.Id) + 1,
//             //     Name: createGameDto.Name,
//             //     Genre: createGameDto.Genre,
//             //     Price: createGameDto.Price,
//             //     ReleaseDate: createGameDto.ReleaseDate
//             // );

//             // games.Add(newGame);

//             // return Results.CreatedAtRoute(
//             //     "GetGameById",
//             //     new { id = newGame.Id },
//             //     newGame
//             // );

//             Game newGame = new()
//             {
//                 Name = createGameDto.Name,
//                 GenreId = createGameDto.GenreId,
//                 Price = createGameDto.Price,
//                 ReleaseDate = createGameDto.ReleaseDate
//             };

//             dbcontext.Games.Add(newGame);
//             await dbcontext.SaveChangesAsync();

//             GameDetailsDto gameDto = new(
//                             newGame.Id,
//                             newGame.Name,
//                             newGame.GenreId,
//                             newGame.Price,
//                             newGame.ReleaseDate
//                         );

//             return Results.CreatedAtRoute(
//                 "GetGameById",
//                 new { id = newGame.Id },
//                 gameDto
//             );



//         });


//         //update existing game
//         group.MapPut("/{id}", async (int id, UpdateGameDto updateGameDto, GameStoreContext dbContext) =>
//         {
//             // var existingGameIndex = games.FindIndex(g => g.Id == id);
//             // if (existingGameIndex == -1)
//             // {

//             var existingGame = await dbContext.Games.FindAsync(id);
//             if (existingGame is null)
//             {
//                 return Results.NotFound();
//             }

//             // var updatedGame = new GameSummaryDto
//             // (
//             //     Id: id,
//             //     Name: updateGameDto.Name,
//             //     Genre: updateGameDto.Genre,
//             //     Price: updateGameDto.Price,
//             //     ReleaseDate: updateGameDto.ReleaseDate
//             // );

//             // games[existingGameIndex] = updatedGame;
//             existingGame.Name = updateGameDto.Name;
//             existingGame.GenreId = updateGameDto.GenreID;
//             existingGame.Price = updateGameDto.Price;
//             existingGame.ReleaseDate = updateGameDto.ReleaseDate;
//             await dbContext.SaveChangesAsync();

//             return Results.NoContent();
//         });

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
