using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models;


public enum TaskStatus
{
    TODO,
    IN_PROGRESS,
    DONE
}
public class Tasks
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.TODO;

    public DateTime CreatedAt { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime UpdatedAt { get; set; }


    // 🔑 Foreign Key
    public int UserId { get; set; }

    // 🔗 Navigation property
    public Users User { get; set; } = null!;
}
