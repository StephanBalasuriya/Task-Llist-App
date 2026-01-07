using System;
using System.Linq;
using WebApplication1.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace WebApplication1.EndPoints;

public static class UserDbEndPoints
{
    public static void MapUserDBEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/dbusers", [Authorize] async (TaskStoreContext db) =>
                    Results.Ok(await db.Users.ToListAsync()));

        app.MapGet("/dbusers/{id}", [Authorize] async (int id, TaskStoreContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            return user is null ? Results.NotFound() : Results.Ok(user);
        });
    }
}