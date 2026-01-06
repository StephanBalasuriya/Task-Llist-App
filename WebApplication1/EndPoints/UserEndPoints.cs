using System;
using WebApplication1.Dtos;

namespace WebApplication1.EndPoints;

public static class UserEndPoints
{
    public static List<UserDto> users = [
    new (1, "alice", "qwertyu", "alice@example.com"),
    new (2, "bob", "asdfghjk", "bob@example.com"),
    new (3, "carol", "zxcvbnm", "carol@example.com"),
    new (4, "david", "uytrewq", "david@example.com"),
    new (5, "eve", "jhgfdsa", "eve@example.com")
];

public static void MapUserEndpoints(this IEndpointRouteBuilder app){
app.MapGet("/", () => "Hello World!");
app.MapGet("/users", () => users);
app.MapGet("/users/{id}", (int id) => {
    var user =users.FirstOrDefault(u => u.UserId == id) ;
    return user is null ? Results.NotFound() : Results.Ok(user);
    });
app.MapPost("/users", (CreateUserDto newUser) =>
{
    if (string.IsNullOrWhiteSpace(newUser.Username))
    {
        return Results.BadRequest(new Dictionary<string, string[]>
        {
            [nameof(newUser.Username)] = ["The Username field is required."]
        });
    }

    var nextId = users.Max(u => u.UserId) + 1;
    var userDto = new UserDto(nextId, newUser.Username.Trim(), newUser.PasswordHash, newUser.Email);
    users.Add(userDto);
    return Results.Created($"/users/{nextId}", userDto);
});
app.MapPut("/users/{id}", (int id, UpdateUserDto updatedUser) =>
{
    var user = users.FirstOrDefault(u => u.UserId == id);
    if (user is null)
    {
        return Results.NotFound();
    }

    if (updatedUser.Username is not null && string.IsNullOrWhiteSpace(updatedUser.Username))
    {
        return Results.BadRequest(new Dictionary<string, string[]>
        {
            [nameof(updatedUser.Username)] = ["The Username field is required."]
        });
    }

    var updatedDto = new UserDto(
        user.UserId,
        (updatedUser.Username ?? user.Username).Trim(),
        updatedUser.PasswordHash ?? user.PasswordHash,
        updatedUser.Email ?? user.Email
    );

    users = users.Where(u => u.UserId != id).ToList();
    users.Add(updatedDto);

    return Results.Ok(updatedDto);
});
app.MapDelete("/users/{id}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.UserId == id);
    if (user is null)
    {
        return Results.NotFound();
    }

    users = users.Where(u => u.UserId != id).ToList();
    return Results.NoContent();
});
}
}
