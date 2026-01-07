using System;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data;

// public class TaskStoreContext(DbContextOptions<TaskStoreContext> options) : DbContext(options)
// {
// public DbSet<Users> Users { get; set; }
// public DbSet<Tasks> Tasks { get; set; }
// }


public class TaskStoreContext : DbContext
{
    public TaskStoreContext(DbContextOptions<TaskStoreContext> options)
        : base(options) { }

 public DbSet<Users> Users { get; set; }
public DbSet<Tasks> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tasks>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}