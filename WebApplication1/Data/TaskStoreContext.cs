using System;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data;

public class TaskStoreContext(DbContextOptions<TaskStoreContext> options) : DbContext(options)
{
public DbSet<Users> Users { get; set; }
public DbSet<Tasks> Tasks { get; set; }
}
