using System;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Dtos;
using WebApplication1.Models;
using BCrypt.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.EndPoints;

public static class Signup_Login
{
    public static void MapSignupLoginEndpoints(this WebApplication app)
    {
        app.MapPost("/signup",[AllowAnonymous] async (CreateUserDto user, TaskStoreContext db) =>
               {
                   if (await db.Users.AnyAsync(u => u.Username == user.Username))
                       return Results.BadRequest("Username already exists.");

                   Users newUser = new()
                   {
                       Username = user.Username,
                       Email = user.Email,
                       PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash)
                   };
                   db.Users.Add(newUser);
                   await db.SaveChangesAsync();
                   return Results.Ok("User created successfully");
               }


        );

        app.MapPost("/login",[AllowAnonymous] async (TaskStoreContext db, UserAuthDto login) =>
    {
        if (string.IsNullOrWhiteSpace(login.Username) || string.IsNullOrWhiteSpace(login.Password))
            return Results.BadRequest("Username and password are required.");

        var jwtSettings = app.Configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings.GetValue<string>("Secret");

        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == login.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
            return Results.Unauthorized();

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secretKey!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
            }),
            Expires = DateTime.UtcNow.AddMinutes(jwtSettings.GetValue<int>("ExpirationMinutes")),
            Issuer = jwtSettings.GetValue<string>("Issuer"),
            Audience = jwtSettings.GetValue<string>("Audience"),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        return Results.Ok(new { Token = jwtToken });
    }    );
        app.MapGet("/profile", [Microsoft.AspNetCore.Authorization.Authorize] () =>
    {
        return Results.Ok("You are authorized!");
    });

        // app.UseAuthentication();
        // app.UseAuthorization();



    }
}
