using System;
using System.Linq;
using WebApplication1.Data;
using Microsoft.EntityFrameworkCore;


namespace WebApplication1.EndPoints;

public static class UserDbEndPoints
{
    public static void MapUserDBEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/dbusers", async (TaskStoreContext db) =>
                    Results.Ok(await db.Users.ToListAsync()));

        app.MapGet("/dbusers/{id}", async (int id, TaskStoreContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            return user is null ? Results.NotFound() : Results.Ok(user);
        });
    }
}