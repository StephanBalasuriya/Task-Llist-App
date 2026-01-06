using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.EndPoints;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

var builder = WebApplication.CreateBuilder(args);

//JWT Settings
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings.GetValue<string>("Secret");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
})
.AddJwtBearer("JwtBearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
        ValidAudience = jwtSettings.GetValue<string>("Audience"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});


//connect to database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TaskStoreContext>(
    options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);


builder.Services.AddAuthorization();

var app = builder.Build();

//Auto create database tables if they do not exist
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TaskStoreContext>();
    dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();
}


app.MapSignupLoginEndpoints();
// app.MapUserEndpoints(); 
app.MapUserDBEndpoints();

app.Run();

public partial class Program { }
