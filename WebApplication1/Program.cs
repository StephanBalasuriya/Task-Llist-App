using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.EndPoints;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TaskStoreContext>(
    options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);
var app = builder.Build();

//Auto create database tables if they do not exist
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TaskStoreContext>();
    dbContext.Database.EnsureCreated();
}



// app.MapUserEndpoints(); 
app.MapUserDBEndpoints();

app.Run();

public partial class Program { }
